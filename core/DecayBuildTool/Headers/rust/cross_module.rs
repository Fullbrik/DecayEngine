pub struct Packet{
    data: Vec<u8>,
    pointer: usize
}

impl Packet{
    pub fn new() -> Packet{
        Packet{data: Vec::new(), pointer: 0}
    }

    pub fn from_data(data: *mut u8, size: usize) -> Packet{
        unsafe{
            let data = Vec::from_raw_parts(data, size, size);

            Packet{data: data, pointer: 0}
        }
    }
}
