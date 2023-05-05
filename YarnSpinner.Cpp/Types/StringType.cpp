#include "StringType.h"

#include "BuiltinTypes.h"
#include "../Value.h"

const std::string Yarn::StringType::Name() const { return "String"; }

std::shared_ptr<Yarn::IType> Yarn::StringType::Parent() const { return BuiltinTypes::Any(); }

const std::string Yarn::StringType::Description() const { return "String"; }

std::string Yarn::StringType::DefaultType() { return ""; }

std::string Yarn::StringType::ToBridgedType(Value value) { return value.ConvertTo<std::string>(); }

std::string Yarn::StringType::MethodConcatenate(Value arg1, Value arg2)
{
    return arg1.ConvertTo<std::string>().append(arg2.ConvertTo<std::string>());
}

bool Yarn::StringType::MethodEqualTo(Value a, Value b)
{
    return a.ConvertTo<std::string>() == b.ConvertTo<std::string>();
}

bool Yarn::StringType::MethodNotEqualTo(Value a, Value b)
{
    return !MethodEqualTo(a, b);
}
