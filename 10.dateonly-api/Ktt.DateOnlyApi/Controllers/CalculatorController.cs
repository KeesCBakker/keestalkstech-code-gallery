using Microsoft.AspNetCore.Mvc;

[ApiController]
public class CalculatorController : ControllerBase
{
    [HttpGet("calc")]
    public MyDateModel Calc(DateOnly input)
    {
        return new MyDateModel
        {
            Stamp = input
        };
    }

    [HttpGet("today")]
    public MyDateModel CalcToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return Calc(today);
    }
}
