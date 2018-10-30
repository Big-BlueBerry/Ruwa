# Grammer

`<Song> = <Metadata> <Line> <Sheet>`

`<Metadata> = <Song name>, <Song composer>, <Sheet composer> <BPM data>`

`<Song name> = "song_name=", <String>`

`<Song composer> = "song_composer=", <Stirng>`

`<Sheet composer> = "sheet_composer=", <String>`

`<BPM data> = <BPM>, { <BPM> }`

`<BPM> = <Bar number>, "~", <BPM of the bar>`

`<Bar number> = <Number>`

`<BPM of the bar> = <Number>`

`<Line> = "-", { "-" }`

`<Sheet> = { <Point> }`

`<Point> = "<Point type>, ",", <Bar>, ",", <Current beat>, ",", <Full beat>, ",", <Position>, ",", <Size>,  [ ":", <Attribute type> { ",", <Attribute type> }]`

`<Point type> = <Number>`

`<Bar> = <Number>`

`<Current beat> = <Number>`

`<Full beat> = <Number>`

`<Position> = <Number>`

`<Size> = <Number>`

`<Digit excluding zero> = "1" | "2" | ... | "9"`

`<Digit> = <Digit excluding zero> | "0"`

`<Number> = <Digit excluding zero>, { <Digit> }`

`<Alphabet> = "a" | "A" | "b" | "B" | ... | "z" | "Z"`

`<String> = '"' { <Alphabet> } '"'`
