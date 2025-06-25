using System;

namespace Mrnchr.Balancery.Statistics.Database
{
  public interface IData
  {
    int ValueType { get; set; }
    float RealValue { get; set; }
    string StringValue { get; set; }

    public object GetValue()
    {
      return ValueType switch
      {
        0 => RealValue,
        1 => StringValue,
        _ => null
      };
    }
    
    public TType GetValue<TType>()
    {
      return ValueType switch
      {
        0 when RealValue is TType value => value,
        1 when StringValue is TType value => value,
        _ => default(TType)
      };
    }

    public void SetValue<TType>(TType value)
    {
      ValueType = typeof(TType) == typeof(float) ? 0 : 1;
      if (ValueType == 0)
      {
        if (value is float floatValue)
          RealValue = floatValue;
        else
          throw new ArgumentException($"Cannot cast expression of type '{typeof(TType)}' to type 'float'");
        StringValue = null;
      }

      if (ValueType == 1)
      {
        StringValue = value as string ?? value.ToString();
        RealValue = 0;
      }
    }
  }
}