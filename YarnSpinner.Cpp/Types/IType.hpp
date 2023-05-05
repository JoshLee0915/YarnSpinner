#pragma once
#include <map>
#include <string>
#include <memory>

namespace Yarn
{
    using MethodCollection = std::map<std::string, void*>;
    
    class IType
    {
    public:
        virtual const std::string Name() const = 0;

        virtual std::shared_ptr<IType> Parent() const = 0;

        virtual const std::string Description() const = 0;

        virtual const std::shared_ptr<MethodCollection> Methods() const = 0;

        virtual ~IType() = 0;
    };
}
