#pragma once
#include "IBridgeableType.hpp"
#include "TypeBase.hpp"
#include "../VirtualMachine.h"

namespace Yarn
{
    class BooleanType: public TypeBase, public IBridgeableType<bool>
    {
    public:
        const std::string Name() const final;
        std::shared_ptr<IType> Parent() const final;
        const std::string Description() const final;
        bool DefaultType();
        bool ToBridgedType(Value value);        
    private:
        static bool MethodEqualTo(Value a, Value b);
        static bool MethodNotEqualTo(Value a, Value b);
        static bool MethodAnd(Value a, Value b);
        static bool MethodOr(Value a, Value b);
        static bool MethodXor(Value a, Value b);
        static bool MethodNot(Value a);
        
        inline static MethodCollection DefaultMethods{
            {OperatorToString(EqualTo), MethodEqualTo},
            {OperatorToString(NotEqualTo), MethodNotEqualTo},
            {OperatorToString(And), MethodAnd},
            {OperatorToString(Or), MethodOr},
            {OperatorToString(Xor), MethodXor},
            {OperatorToString(Not), MethodNot},
        };
    public:
        BooleanType(): TypeBase(&DefaultMethods) {}
    };
}

