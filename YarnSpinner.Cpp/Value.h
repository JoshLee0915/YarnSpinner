#pragma once
#include <any>
#include <memory>

#include "Types/IBridgeableType.hpp"
#include "Types/IType.hpp"

namespace Yarn
{
    class Value
    {
    private:
        std::shared_ptr<IType> _type;
    public:
        std::shared_ptr<IType> Type();
        std::any InternalValue;

        Value(const Value& value): _type{ value._type }, InternalValue{ value.InternalValue } {}

        template<class TBridgeableType>
        Value(IBridgeableType<TBridgeableType>& type): _type{type}, InternalValue{ type.DefaultType() } {}

        template<class T>
        Value(IType& type, T internalValue): _type{&type}, InternalValue{ internalValue } {}

        int CompareTo(std::any obj);

        template<class T>
        T ConvertTo()
        {
            return std::any_cast<T>(InternalValue);
        }

        std::string ToString();
    };
}

