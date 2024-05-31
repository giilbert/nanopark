use std::{
    collections::HashMap,
    sync::{
        mpsc::{self, TryRecvError},
        Arc,
    },
    thread::JoinHandle,
    time::Duration,
};

use nom::{
    bytes::complete::tag,
    character::complete::digit1,
    combinator::{map, opt, recognize},
    multi::separated_list0,
    sequence::tuple,
};
use parking_lot::Mutex;
use serde::Serialize;
use serialport::SerialPort;

#[derive(Debug, Clone)]
pub enum Command {
    DoUpdate,
}

#[derive(Debug, Clone, PartialEq, Eq, Serialize)]
pub struct Controller {
    pub id: usize,
    pub x: i8,
    pub y: i8,
    pub is_pressed: bool,
}

fn parse_packet_1(data: &[u8]) -> nom::IResult<&[u8], Vec<Controller>> {
    let parse_integer = || {
        map(
            tuple((opt(tag(['-' as u8])), recognize(digit1))),
            |(sign, number): (Option<&[u8]>, &[u8])| {
                let string = number
                    .iter()
                    .map(|&letter| (letter as char).to_string())
                    .collect::<Vec<String>>()
                    .join("");

                let number = string.parse::<i32>().expect("unparseable string");
                return if sign.is_some() { -number } else { number };
            },
        )
    };
    let comma = || tag([',' as u8]);

    map(
        separated_list0(
            tag(['\n' as u8]),
            tuple((
                parse_integer(),
                comma(),
                parse_integer(),
                comma(),
                parse_integer(),
                comma(),
                parse_integer(),
            )),
        ),
        |items: Vec<(i32, &[u8], i32, &[u8], i32, &[u8], i32)>| {
            items
                .iter()
                .map(|(id, _, x, _, y, _, is_button_pressed)| Controller {
                    id: *id as usize,
                    x: *x as i8,
                    y: *y as i8,
                    is_pressed: *is_button_pressed == 1,
                })
                .collect()
        },
    )(data)
}

#[derive(Debug, Default)]
pub struct State {
    pub controllers: HashMap<usize, Controller>,
}

fn process_buffer(buffer: &mut Vec<u8>, state: &Mutex<State>) {
    let mut end_offset = 0usize;

    for (index, &data) in buffer.iter().enumerate() {
        if data == 0x0 && index != 0 {
            end_offset = index;
        }
    }

    let data = buffer.drain(0..=end_offset).collect::<Vec<u8>>();
    let packet_type = match data.get(1) {
        Some(&packet_type) => packet_type,
        None => return,
    };

    match packet_type {
        1 => {
            let (_rest, data) = parse_packet_1(&data[2..(data.len() - 1)]).unwrap();
            state.lock().controllers = data.into_iter().map(|c| (c.id, c)).collect();
        }
        _ => {
            println!("Unknown packet type: {}", packet_type);
        }
    }
}

fn run(state: Arc<Mutex<State>>, rx: mpsc::Receiver<Command>, mut port: Box<dyn SerialPort>) {
    let mut buffer = Vec::new();

    loop {
        std::thread::park();

        let ready_bytes = port.bytes_to_read().expect("error getting bytes to read") as usize;
        if ready_bytes > 0 {
            let mut vec = vec![0u8; ready_bytes];
            port.read(&mut vec).expect("error reading");
            buffer.append(&mut vec);

            process_buffer(&mut buffer, state.as_ref());
        }

        match rx.try_recv() {
            Ok(Command::DoUpdate) => {
                port.write(&[0x1]).expect("error writing to serial port");
            }
            Err(TryRecvError::Empty) => (),
            Err(TryRecvError::Disconnected) => break,
        }
    }
}

pub struct SerialController {
    thread: Option<(mpsc::Sender<Command>, JoinHandle<()>)>,
    pub state: Arc<Mutex<State>>,
}

impl SerialController {
    pub fn new() -> Self {
        Self {
            thread: None,
            state: Arc::new(Mutex::new(State::default())),
        }
    }

    pub fn list_ports(&self) -> Vec<String> {
        serialport::available_ports()
            .expect("No serial ports found")
            .iter()
            .map(|port| port.port_name.clone())
            .collect()
    }

    pub fn poll(&mut self) {
        if let Some((tx, handle)) = &self.thread {
            // godot_print!(".");
            // tx.send(Command::DoUpdate).expect("error send command");
            let _ = tx.send(Command::DoUpdate);
            handle.thread().unpark();
        }
    }

    pub fn disconnect(&mut self) {
        if let Some(thread_data) = self.thread.take() {
            drop(thread_data)
        }
    }

    pub fn connect(&mut self, port: &str) {
        let mut port = serialport::new(port, 115200)
            .open()
            .expect("Failed to open serial port");

        port.set_timeout(Duration::from_secs(60))
            .expect("error setting timeout");

        let (tx, rx) = mpsc::channel();
        let state = self.state.clone();

        let handle = std::thread::spawn(move || run(state, rx, port));

        self.thread = Some((tx, handle));
    }
}

#[cfg(test)]
mod tests {
    use crate::serial::Controller;

    use super::parse_packet_1;

    #[test]
    pub fn parse_packet_1_test() {
        assert_eq!(
            parse_packet_1(b"1,-1,2,1\n").unwrap().1,
            vec![Controller {
                id: 1,
                x: -1,
                y: 2,
                is_pressed: true
            }]
        );

        assert_eq!(
            parse_packet_1(b"1,-110,113,0\n2,0,111,1\n").unwrap().1,
            vec![
                Controller {
                    id: 1,
                    x: -110,
                    y: 113,
                    is_pressed: false
                },
                Controller {
                    id: 2,
                    x: 0,
                    y: 111,
                    is_pressed: true
                }
            ]
        );
    }
}
