#pragma once
#include "IType.hpp"

namespace Yarn
{
    class Value;
    
    template<class TBridgedType>
    class IBridgeableType: public virtual IType
    {
    public:
        virtual TBridgedType DefaultType() = 0;

        virtual TBridgedType ToBridgedType(Value value) = 0;
    };
}
