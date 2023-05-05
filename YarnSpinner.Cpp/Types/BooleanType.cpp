#include "BooleanType.h"

#include "BuiltinTypes.h"
#include "../Value.h"

const std::string Yarn::BooleanType::Name() const { return "Bool"; }

std::shared_ptr<Yarn::IType> Yarn::BooleanType::Parent() const { return BuiltinTypes::Any(); }

const std::string Yarn::BooleanType::Description() const { return "Bool"; }

bool Yarn::BooleanType::DefaultType() { return false; }

bool Yarn::BooleanType::ToBridgedType(Value value) { return value.ConvertTo<bool>(); }

bool Yarn::BooleanType::MethodEqualTo(Value a, Value b) { return a.ConvertTo<bool>() == b.ConvertTo<bool>(); }

bool Yarn::BooleanType::MethodNotEqualTo(Value a, Value b) { return !MethodEqualTo(a, b); }

bool Yarn::BooleanType::MethodAnd(Value a, Value b) { return a.ConvertTo<bool>() && b.ConvertTo<bool>(); }

bool Yarn::BooleanType::MethodOr(Value a, Value b) { return a.ConvertTo<bool>() || b.ConvertTo<bool>(); }

bool Yarn::BooleanType::MethodXor(Value a, Value b) { return a.ConvertTo<bool>() ^ b.ConvertTo<bool>(); }

bool Yarn::BooleanType::MethodNot(Value a) { return !a.ConvertTo<bool>(); }
