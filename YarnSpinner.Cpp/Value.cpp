#include "Value.h"

int Yarn::Value::CompareTo(std::any obj)
{
    return 0;
}

std::string Yarn::Value::ToString()
{
    // TODO: might need to any_cast string
    std::string value = "unknown";
    if(const auto v = std::any_cast<int>(InternalValue))
        value = std::to_string(v);
    if(const auto v = std::any_cast<unsigned int>(InternalValue))
        value = std::to_string(v);
    else if(const auto v = std::any_cast<float>(InternalValue))
        value = std::to_string(v);
    else if(const auto v = std::any_cast<double>(InternalValue))
        value = std::to_string(v);
    else if(const auto v = std::any_cast<bool>(InternalValue))
        value = std::to_string(v);
    else if(const auto v = std::any_cast<char*>(InternalValue))
        value = std::string(v);
    return "[Value: type=" + Type()->Name() + ", value=" + value + "]";
}
