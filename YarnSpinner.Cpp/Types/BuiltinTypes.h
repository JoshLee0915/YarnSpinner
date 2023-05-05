#pragma once
#include <functional>
#include <typeindex>

#include "IType.hpp"

namespace Yarn
{
    class BuiltinTypes
    {
    public:
        static std::shared_ptr<IType> Undefined();
        static std::shared_ptr<IType> String();
        static std::shared_ptr<IType> Number();
        static std::shared_ptr<IType> Boolean();
        static std::shared_ptr<IType> Any();

        static const std::shared_ptr<IType> TypeMappings(std::type_index type);

        inline static const std::vector<std::function<std::shared_ptr<IType>()>> AllBuiltinTypes{
            Undefined,
            String,
            Number,
            Boolean,
            Any,
        };
    };
}

