#include "BuiltinTypes.h"

#include <stdexcept>

#include "AnyType.hpp"
#include "BooleanType.h"

std::shared_ptr<Yarn::IType> Yarn::BuiltinTypes::Undefined()
{
    return nullptr;
}

std::shared_ptr<Yarn::IType> Yarn::BuiltinTypes::String()
{
    // TODO: Implement String
    throw std::logic_error("Not Implemented");
}

std::shared_ptr<Yarn::IType> Yarn::BuiltinTypes::Number()
{
    // TODO: Implement Number
    throw std::logic_error("Not Implemented");
}

std::shared_ptr<Yarn::IType> Yarn::BuiltinTypes::Boolean()
{
    static std::shared_ptr<IType> instance = std::make_shared<BooleanType>();
    return instance;
}

std::shared_ptr<Yarn::IType> Yarn::BuiltinTypes::Any()
{
    static std::shared_ptr<IType> instance = std::make_shared<AnyType>();
    return instance;
}

const std::shared_ptr<Yarn::IType> Yarn::BuiltinTypes::TypeMappings(std::type_index type)
{
    switch (type)
    {
    case typeid(std::string):
        return String();
    case typeid(bool):
        return Boolean();
    case typeid(int):
    case typeid(unsigned int):
    case typeid(float):
    case typeid(double):
    case typeid(std::byte):
    case typeid(char):
    case typeid(unsigned char):
    case typeid(short):
    case typeid(unsigned short):
    case typeid(long):
    case typeid(unsigned long):
    case typeid(int8_t):    
    case typeid(uint8_t):    
    case typeid(int16_t):    
    case typeid(uint16_t):    
    case typeid(int32_t):    
    case typeid(uint32_t):    
    case typeid(int64_t):    
    case typeid(uint64_t):    
        return Number();
    default:
        return Any();
    }
}
