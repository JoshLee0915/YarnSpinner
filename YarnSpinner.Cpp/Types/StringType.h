#pragma once
#include "IBridgeableType.hpp"
#include "TypeBase.hpp"
#include "../VirtualMachine.h"

namespace Yarn
{
    class StringType: TypeBase, IBridgeableType<std::string>
    {
    public:
        const std::string Name() const override;
        std::shared_ptr<IType> Parent() const override;
        const std::string Description() const override;
        std::string DefaultType() override;
        std::string ToBridgedType(Value value) override;
    private:
        static std::string MethodConcatenate(Value arg1, Value arg2);
        static bool MethodEqualTo(Value a, Value b);
        static bool MethodNotEqualTo(Value a, Value b);

        inline static MethodCollection DefaultMethods{
                {OperatorToString(EqualTo), MethodEqualTo},
                {OperatorToString(NotEqualTo), MethodNotEqualTo},
                {OperatorToString(Add), MethodConcatenate},
        };
    public:
        StringType(): TypeBase(&DefaultMethods) {}
    };
}

