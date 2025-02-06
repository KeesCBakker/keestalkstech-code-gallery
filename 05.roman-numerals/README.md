# Roman Numerals
Parsing Roman Numerals in C# is a good way to explore
(implicit) operator overloading.

More information can be found in these blogs:
- <a href="https://keestalkstech.com/2017/08/parsing-roman-numerals-using-csharp/">Parsing Roman Numerals using C##</a>
- <a href="https://keestalkstech.com/2017/08/calculations-with-roman-numerals-in-csharp/">Calculations with Roman Numerals using C#</a>

## Examples
I mean, this looks pretty cool, right?

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
