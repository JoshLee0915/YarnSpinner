#pragma once
#include "IType.hpp"

namespace Yarn
{
    class TypeBase: public virtual IType
    {
    private:
        std::shared_ptr<MethodCollection> methods = std::make_shared<MethodCollection>();
    public:
        const std::shared_ptr<MethodCollection> Methods() const override { return methods; }
        
    protected:
        TypeBase(MethodCollection* methods)
        {
            if(methods == nullptr)
                return;
            
            for(auto const& [key, val]: *methods)
                this->methods->emplace(key, val);                
        }
    };
}
