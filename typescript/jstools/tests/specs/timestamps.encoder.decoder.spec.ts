import tools = require('../../tools');

var randomTimeStamps: Date[] = [];

var prevDate = new Date(2015, 0, 1, 12, 0, 0);
var pushTimeStamp = (array: Date[], offset: number) => {
    array.push((prevDate = new Date(prevDate.getTime() + offset * 1000)));
};

var count = (20 * Math.random()) << 0;
for (var i = 0; i < count; i++) {
    pushTimeStamp(randomTimeStamps, (100 * Math.random()) << 0);
}

var encodedRandomTimeStamps = tools.convert.encodeTimeStamps(randomTimeStamps);

describe('Encoding and decoding random timestamps',() => {
    it('encoded random timestamps should be decoded to original input',() => {
        expect(tools.convert.decodeTimeStamps(encodedRandomTimeStamps)).toEqual(randomTimeStamps);
    })
});
