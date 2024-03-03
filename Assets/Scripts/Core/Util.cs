using System;

namespace Core
{
    //shared??
    public enum TypeCompare
    {
        None,
        GreaterThan, //>
        LessThan,    //<
        Equal,       //==
        NotEqual,    //!=
    }
    
    public static class UtilExtended
    {
        private static readonly float FloatingPointComparisonTolerance = 0.01f;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyValue">Реальное значение</param>
        /// <param name="typeCompare">Операция сравнения</param>
        /// <param name="compareValue">Значение из конфига</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool CheckCompareIsTrue(this float propertyValue, TypeCompare typeCompare, float compareValue)
        {
            return typeCompare switch
            {
                TypeCompare.GreaterThan => propertyValue > compareValue,
                TypeCompare.LessThan => propertyValue < compareValue,
                TypeCompare.Equal => Math.Abs(propertyValue - compareValue) < FloatingPointComparisonTolerance,
                TypeCompare.NotEqual => Math.Abs(propertyValue - compareValue) > FloatingPointComparisonTolerance,
                _ => throw new ArgumentOutOfRangeException(nameof(typeCompare), typeCompare, null)
            };
        }
    }
}