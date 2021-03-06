# base64-timestamps
Encodes/decodes series of timestamps to/from Base64 strings.

If you need to send long series of timestamps to the browser and back,
then you might find this tool usefool.
It can encode arrays of date instances to much shorter Base64 string
representations, and vica versa, thus saving bandwith on the wire.
Encoding and decoding is supported in both C# and JavaScript, so it
can be used in ASP.NET or NodeJS/browser environment also.
The idea behind encoding is similar to wrapping polyline coordinates like
Google/Bing maps do, it only encodes differences from the initial
value. For more details, see the comments in the C# sourcecodes.

#### Solution

The TypeScript and C# source codes are contained in a Visual Studio 2015 solution,
more specially in separeted projects: one project for the TypeScript codes and
Jasmine tests, and two other projects for the C# codes and Moq/MSPEC tests.

#### Usage examples

Encode timestamps:

**C#**
```c#
using rblt.Tools;

System.DateTime[] timestamps = new System.DateTime[] { ... };
string encTimeStamps = Convert.EncodeTimeStamps( timestamps );
```
**Javascript**
```js
require(['path/to/tools'], function(tools) {
  var timestamps = [ new Date(...), ..., new Date(...) ];
  var encTimeStamps = tools.Convert.encodeTimeStamps( timestamps );
});
```
Decode timestamps:

**C#**
```c#
using rblt.Tools;

string encTimeStamps = "QPM3HH8gKMD///eP/x8AAP//AAD///8/";
DateTime[] timeStamps = Convert.DecodeTimeStamps( encTimeStamps );
```
**Javascript**
```js
require(['path/to/tools'], function(tools) {
  var encTimeStamps = 'QPM3HH8gKMD///eP/x8AAP//AAD///8/';
  var timestamps = tools.Convert.decodeTimeStamps( encTimeStamps );
});
```

#### Remarks

Encoder algorithm accepts monotonic timestamp series only. This is by design.

#### Todos

Because the algorithm accepts monotonic timestamp series, it might be a problem
when the clock is set backward according to DST, thus these time series won't be
monotonic. The algorithm should be modified to handle these situations.
