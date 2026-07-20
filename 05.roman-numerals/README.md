# Roman Numerals

Parsing Roman Numerals in C# is a good way to explore
(implicit) operator overloading.

<a href="https://keestalkstech.com/parsing-roman-numerals-using-csharp/">Read the blog on KeesTalksTech: Parsing Roman Numerals using C#</a>
<a href="https://keestalkstech.com/calculations-with-roman-numerals-in-csharp/">Read the blog on KeesTalksTech: Calculations with Roman Numerals using C#</a>

## Features

- Support for .NET 10
- Support for parsing Roman numerals from strings
- Support for implicit operators (string to RomanNumeral and back)
- Support for math operators (addition, subtraction)
- Support for comparison operators
- Support for casting between RomanNumeral and int

## Examples

```csharp
RomanNumeral I = "I";
RomanNumeral IV = "IV";

int a = IV - 1;
int b = 4 - I;
int c = IV - "I";
int d = "IV" - I;
int e = IV - I;

string f = IV - 1;
string g = 4 - I;
string h = IV - "I";
string i = "IV" - I;
string j = IV - I;

RomanNumeral k = IV - 1;
RomanNumeral l = 4 - I;
RomanNumeral m = IV - "I";
RomanNumeral n = "IV" - I;
RomanNumeral o = IV - I;
```
