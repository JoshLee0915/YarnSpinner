#pragma once

#include <vector>

#include "IType.hpp"

namespace Yarn
{
    class FunctionType: IType
    {
    public:
        std::shared_ptr<IType> ReturnType;
        std::pmr::vector<std::shared_ptr<IType>> Parameters{};
        
        const std::string Name() const override;
        std::shared_ptr<IType> Parent() const override;
        const std::string Description() const override;
        const std::shared_ptr<MethodCollection> Methods() const override;
        void AddParameter(std::shared_ptr<IType> parameterType);
    };
}
