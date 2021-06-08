pub struct Packet {
    pub data: Vec<u8>,
    pointer: usize,
}

impl Packet {
    pub fn new() -> Packet {
        Packet {
            data: Vec::new(),
            pointer: 0,
        }
    }

    pub fn from_data(data: *mut u8, size: usize) -> Packet {
        unsafe {
            let data = Vec::from_raw_parts(data, size, size);

            Packet {
                data: data,
                pointer: 0,
            }
        }
    }

    pub fn get_data(&self) -> &Vec<u8> {
        &self.data
    }

    pub fn push_u8(&mut self, val: u8) {
        self.data.insert(self.pointer, val);
        self.pointer += 1;
    }

    pub fn push_i8(&mut self, val: i8) {
        self.push_u8(val as u8);
    }

    pub fn push_u16(&mut self, val: u16) {
        self.push_u8((val >> 8) as u8);
        self.push_u8(val as u8);
    }

    pub fn push_i16(&mut self, val: i16) {
        self.push_u16(val as u16);
    }

    pub fn pull_u8(&mut self) -> u8 {
        let data = self.data[self.pointer];
        self.pointer += 1;

        return data;
    }

    pub fn pull_i8(&mut self) -> i8 {
        self.pull_u8() as i8
    }

    pub fn pull_u16(&mut self) -> u16 {
        let mut val = 0;
        val += (self.pull_u8() as u16) << 8;
        val += self.pull_u8() as u16;
        val
    }

    pub fn pull_i16(&mut self) -> i16 {
        self.pull_u16() as i16
    }
}
