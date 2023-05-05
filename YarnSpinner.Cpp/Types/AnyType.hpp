#pragma once
#include <memory>

#include "IType.hpp"

namespace Yarn
{
    class AnyType: public IType
    {
    public:
        const std::string Name() const final { return "Any"; }
        std::shared_ptr<IType> Parent() const final { return nullptr; }
        const std::string Description() const final { return "Any type."; }
        const std::shared_ptr<MethodCollection> Methods() const final { return nullptr; }
    };
}
