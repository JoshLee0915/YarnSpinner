#include "NumberType.h"

#include "BuiltinTypes.h"
#include "../Value.h"

const std::string Yarn::NumberType::Name() const { return "Number"; }

std::shared_ptr<Yarn::IType> Yarn::NumberType::Parent() const { return BuiltinTypes::Any(); }

const std::string Yarn::NumberType::Description() const { return "Number"; }

float Yarn::NumberType::DefaultType() { return 0.0f; }

float Yarn::NumberType::ToBridgedType(Value value) { return value.ConvertTo<float>(); }

bool Yarn::NumberType::MethodEqualTo(Value a, Value b)
{
    return a.ConvertTo<float>() == b.ConvertTo<float>();
}

bool Yarn::NumberType::MethodNotEqualTo(Value a, Value b)
{
    return !MethodEqualTo(a, b);
}

float Yarn::NumberType::MethodAdd(Value a, Value b)
{
    return a.ConvertTo<float>() + b.ConvertTo<float>();
}

float Yarn::NumberType::MethodSubtract(Value a, Value b)
{
    return a.ConvertTo<float>() - b.ConvertTo<float>();
}

float Yarn::NumberType::MethodDivide(Value a, Value b)
{
    return a.ConvertTo<float>() / b.ConvertTo<float>();
}

float Yarn::NumberType::MethodMultiply(Value a, Value b)
{
    return a.ConvertTo<float>() * b.ConvertTo<float>();
}

int Yarn::NumberType::MethodModulus(Value a, Value b)
{
    return a.ConvertTo<int>() % b.ConvertTo<int>();
}

float Yarn::NumberType::MethodUnaryMinus(Value a)
{
    return -a.ConvertTo<float>();
}

bool Yarn::NumberType::MethodGreaterThan(Value a, Value b)
{
    return a.ConvertTo<float>() > b.ConvertTo<float>();
}

bool Yarn::NumberType::MethodGreaterThanOrEqualTo(Value a, Value b)
{
    return a.ConvertTo<float>() >= b.ConvertTo<float>();
}

bool Yarn::NumberType::MethodLessThan(Value a, Value b)
{
    return a.ConvertTo<float>() < b.ConvertTo<float>();
}

bool Yarn::NumberType::MethodLessThanOrEqualTo(Value a, Value b)
{
    return a.ConvertTo<float>() <= b.ConvertTo<float>();
}

