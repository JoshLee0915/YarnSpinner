#pragma once
#include <stdexcept>

namespace Yarn
{
    /// <summary>
    /// Lists the available operators that can be used with Yarn values.
    /// </summary>
    enum Operator
    {
        /// <summary>A unary operator that returns its input.</summary>
        None,

        /// <summary>A binary operator that represents equality.</summary>
        EqualTo,

        /// <summary>A binary operator that represents a value being
        /// greater than another.</summary>
        GreaterThan,

        /// <summary>A binary operator that represents a value being
        /// greater than or equal to another.</summary>
        GreaterThanOrEqualTo,

        /// <summary>A binary operator that represents a value being less
        /// than another.</summary>
        LessThan,

        /// <summary>A binary operator that represents a value being less
        /// than or equal to another.</summary>
        LessThanOrEqualTo,

        /// <summary>A binary operator that represents
        /// inequality.</summary>
        NotEqualTo,

        /// <summary>A binary operator that represents a logical
        /// or.</summary>
        Or,

        /// <summary>A binary operator that represents a logical
        /// and.</summary>
        And,

        /// <summary>A binary operator that represents a logical exclusive
        /// or.</summary>
        Xor,

        /// <summary>A binary operator that represents a logical
        /// not.</summary>
        Not,

        /// <summary>A unary operator that represents negation.</summary>
        UnaryMinus,

        /// <summary>A binary operator that represents addition.</summary>
        Add,

        /// <summary>A binary operator that represents
        /// subtraction.</summary>
        Minus,

        /// <summary>A binary operator that represents
        /// multiplication.</summary>
        Multiply,

        /// <summary>A binary operator that represents division.</summary>
        Divide,

        /// <summary>A binary operator that represents the remainder
        /// operation.</summary>
        Modulo,
    };

    inline std::string OperatorToString(Operator opr)
    {
        switch (opr)
        {
            case None:                  return "None";
            case EqualTo:               return "EqualTo";
            case GreaterThan:           return "GreaterThan";
            case GreaterThanOrEqualTo:  return "GreaterThanOrEqualTo";
            case LessThan:              return "LessThan";
            case LessThanOrEqualTo:     return "LessThanOrEqualTo";
            case NotEqualTo:            return "NotEqualTo";
            case Or:                    return "Or";
            case And:                   return "And";
            case Xor:                   return "Xor";
            case Not:                   return "Not";
            case UnaryMinus:            return "UnaryMinus";
            case Add:                   return "Add";
            case Minus:                 return "Minus";
            case Multiply:              return "Multiply";
            case Divide:                return "Divide";
            case Modulo:                return "Modulo";
            default:                    throw std::invalid_argument("Unimplemented Operator");
        }
    }
    
    class VirtualMachine
    {
    public:
    
    };
}

