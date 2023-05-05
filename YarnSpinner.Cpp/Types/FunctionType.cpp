#include "FunctionType.h"

#include "BuiltinTypes.h"

const std::string Yarn::FunctionType::Name() const { return "Function"; }

std::shared_ptr<Yarn::IType> Yarn::FunctionType::Parent() const { return BuiltinTypes::Any(); }

const std::string Yarn::FunctionType::Description() const
{
    std::string description;
    for(const auto& param: Parameters)
    {
        if(param != nullptr)
            description += "Undefined, ";
        else
            description += param->Name() + ", ";
    }
    description.erase(description.end()-2, description.end());

    const std::string returnTypeName = ReturnType != nullptr ? ReturnType->Name() : "Undefined";
    return "(" + description + ") -> " + returnTypeName;  
}

const std::shared_ptr<Yarn::MethodCollection> Yarn::FunctionType::Methods() const { return nullptr; }

void Yarn::FunctionType::AddParameter(std::shared_ptr<IType> parameterType)
{
    Parameters.emplace_back(parameterType);
}
