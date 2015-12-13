export module convert {

    /**
    * Decodes a Base64 encododed timestamp series.
    * 
    * @param encodedStr Encoded timestamps
    * @return Array of Date instances
    */
    export function decodeTimeStamps(encodedStr: string): Date[] {
        if (!encodedStr) return [];

        var stream = atob(encodedStr);
        if (stream.length % 2 > 0) throw 'InvalidInput';

        var readBlock = (stream: string, position: number = 0): number => {
            var bytes: number = 0;
            for (var j = 0; j < 4; j++)
                bytes |= (stream.charCodeAt(position + j) << (j * 8));
            return bytes >>> 0;
        };

        var head = readBlock(stream);
        var beginDate = new Date(new Date(2000, 0, 1).getTime() + (head * 1000));

        var timeStamps: Date[] = new Array();
        timeStamps.push(beginDate);

        var getDiffs = (bytes: number): number[]=> {
            var flag = bytes >>> 30,
                blocks = flag + 1,
                blocksize = (30 / blocks) << 0,
                mask = Math.pow(2, blocksize) - 1;

            var result: number[] = new Array(blocks);
            for (var i = 0; i < blocks; i++)
                result[(blocks - 1) - i] = (bytes >>> (i * blocksize)) & mask;
            return result;
        };

        var prevDate = beginDate,
            i = 4;
        while (i < stream.length) {
            var bytes = readBlock(stream, i);
            var diffs = getDiffs(bytes);
            for (var k = 0; k < diffs.length; k++)
                timeStamps.push((prevDate = new Date(prevDate.getTime() + (diffs[k] * 1000))));
            i += 4;
        }

        return timeStamps;
    }

    /**
    * Encodes timestamp series into a Base64 string.
    * 
    * @param items Timestamps to encode
    * @return Base64 encoded timestamps
    */
    export function encodeTimeStamps(items: Date[]): string {
        if (items.length == 0) return null;

        var diffs: number[] = [];

        var i = 1,
            buckets = new Array<number>(4);
        while (i < items.length) {
            var bucketPtr = 0,
                flag = 4;

            var diff = ((items[i].getTime() - items[i - 1].getTime()) / 1000) << 0;
            do {
                if (diff < (1 << (30 / flag))) {
                    buckets[bucketPtr] = diff;
                    bucketPtr++;
                    i++;

                    if (i < items.length) {
                        if (items[i - 1] > items[i])
                            throw 'NegativeDifference';
                        else
                            diff = ((items[i].getTime() - items[i - 1].getTime()) / 1000) << 0;
                    }
                }
                else {
                    flag--;
                }
            }
            while (flag > 0 && bucketPtr < flag && i < items.length);

            if (flag == 0)
                throw 'FlagOverflow';

            var block = 0;
            for (var j = 0; j < bucketPtr; j++)
                block |= buckets[bucketPtr - 1 - j] << (((30 / bucketPtr) << 0) * j);

            block = (block | (bucketPtr - 1) << 30) >>> 0;

            diffs.push(block);
        }

        var getBytes = (num: number): number[]=> {
            var arr = [
                (num >>> 0) & 0x000000ff,
                (num >>> 8) & 0x000000ff,
                (num >>> 16) & 0x000000ff,
                (num >>> 24) & 0x000000ff
            ];
            return arr;
        };

        var startDay = ((items[0].getTime() - new Date(2000, 0, 1, 0, 0, 0, 0).getTime()) / 1000) << 0,
            buffer = getBytes(startDay);

        diffs.forEach(d => buffer.push.apply(buffer, getBytes(d)));

        var result = '';
        buffer.forEach(b => result += String.fromCharCode(b));

        return window.btoa(result);
    }
}
