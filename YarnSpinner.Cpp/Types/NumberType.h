#pragma once
#include "IBridgeableType.hpp"
#include "TypeBase.hpp"
#include "../VirtualMachine.h"

namespace Yarn
{
    class NumberType: TypeBase, IBridgeableType<float>
    {
    public:
        const std::string Name() const override;
        std::shared_ptr<IType> Parent() const override;
        const std::string Description() const override;
        float DefaultType() override;
        float ToBridgedType(Value value) override;
    private:
        static bool MethodEqualTo(Value a, Value b);
        static bool MethodNotEqualTo(Value a, Value b);
        static float MethodAdd(Value a, Value b);
        static float MethodSubtract(Value a, Value b);
        static float MethodDivide(Value a, Value b);
        static float MethodMultiply(Value a, Value b);
        static int MethodModulus(Value a, Value b);
        static float MethodUnaryMinus(Value a);
        static bool MethodGreaterThan(Value a, Value b);
        static bool MethodGreaterThanOrEqualTo(Value a, Value b);
        static bool MethodLessThan(Value a, Value b);
        static bool MethodLessThanOrEqualTo(Value a, Value b);
        
        inline static MethodCollection DefaultMethods{
            {OperatorToString(EqualTo), MethodEqualTo},
            {OperatorToString(NotEqualTo), MethodNotEqualTo},
            {OperatorToString(Add), MethodAdd},
            {OperatorToString(Minus), MethodSubtract},
            {OperatorToString(Divide), MethodDivide},
            {OperatorToString(Multiply), MethodMultiply},
            {OperatorToString(Modulo), MethodModulus},
            {OperatorToString(UnaryMinus), MethodUnaryMinus},
            {OperatorToString(GreaterThan), MethodGreaterThan},
            {OperatorToString(GreaterThanOrEqualTo), MethodGreaterThanOrEqualTo},
            {OperatorToString(LessThan), MethodLessThan},
            {OperatorToString(LessThanOrEqualTo), MethodLessThanOrEqualTo},
        };
    public:
        NumberType(): TypeBase(&DefaultMethods) {}
    };
}
