import tools = require('../../tools');

var encodedTimeStamps1 = 'QPM3HISBIMCIg6HACQAAAA==';
var originalTimeStamps1: Date[] = [];

var prevDate = new Date(2015, 0, 1, 12, 0, 0);
var pushTimeStamp = (array: Date[], offset: number) => {
    array.push((prevDate = new Date(prevDate.getTime() + offset * 1000)));
};

for (var i = 0; i < 10; i++) {
    pushTimeStamp(originalTimeStamps1, i);
}


prevDate = new Date(2015, 0, 1, 12, 0, 0);
var encodedTimeStamps2 = 'QPM3HH8gKMD///eP/x8AAP//AAD///8/';
var originalTimeStamps2: Date[] = [];

// heading
originalTimeStamps2.push(prevDate);
// 4 slots (7 bits) block
pushTimeStamp(originalTimeStamps2, 1);
pushTimeStamp(originalTimeStamps2, 32);
pushTimeStamp(originalTimeStamps2, 64);
pushTimeStamp(originalTimeStamps2, 127);
//// 3 slots (10 bits) block
pushTimeStamp(originalTimeStamps2, 255);
pushTimeStamp(originalTimeStamps2, 511);
pushTimeStamp(originalTimeStamps2, 1023);
// 2 slots (15 bits) block
pushTimeStamp(originalTimeStamps2, 8191);
pushTimeStamp(originalTimeStamps2, 65535);
// 1 slot (30 bits) block
pushTimeStamp(originalTimeStamps2, 1073741823);


prevDate = new Date(2015, 0, 1, 12, 0, 0);
var encodedTimeStamps3 = 'QPM3HH8gKMD///GH/wMAAP//AAD//wAA////PwEAAAA=';
var originalTimeStamps3: Date[] = [];

originalTimeStamps3.push(prevDate);
// ^^ heading
pushTimeStamp(originalTimeStamps3, 1);
pushTimeStamp(originalTimeStamps3, 32);
pushTimeStamp(originalTimeStamps3, 64);
pushTimeStamp(originalTimeStamps3, 127);
// ^^ should be the 1st block ( 4 x 7 bits )
pushTimeStamp(originalTimeStamps3, 127);
pushTimeStamp(originalTimeStamps3, 127);
pushTimeStamp(originalTimeStamps3, 1023);
// ^^ should be the 2nd block ( 3 x 10 bits )
pushTimeStamp(originalTimeStamps3, 1023);
pushTimeStamp(originalTimeStamps3, 65535);
// ^^ should be the 3rd block ( 2 x 15 bits )
pushTimeStamp(originalTimeStamps3, 65535);
// ^^ should be the 4th block ( 1 x 30 bits )
pushTimeStamp(originalTimeStamps3, 1073741823);
// ^^ should be the 5th block ( 1 x 30 bits )
pushTimeStamp(originalTimeStamps3, 1);
// ^^ should be the last block ( 1 x 30 bits )


describe('Decoding timestamps',() => {
    it('encoded timestamps1 should be decodeable correctly',() => {
        expect(tools.convert.decodeTimeStamps(encodedTimeStamps1)).toEqual(originalTimeStamps1);
    })

    it('encoded timestamps2 should be decodeable correctly',() => {
        expect(tools.convert.decodeTimeStamps(encodedTimeStamps2)).toEqual(originalTimeStamps2);
    })

    it('encoded timestamps3 should be decodeable correctly',() => {
        expect(tools.convert.decodeTimeStamps(encodedTimeStamps3)).toEqual(originalTimeStamps3);
    })
});
