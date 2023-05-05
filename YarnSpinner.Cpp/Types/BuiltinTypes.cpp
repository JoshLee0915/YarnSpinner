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

std::shared_ptr<Yarn::IType> Yarn::BuiltinTypes::TypeMappings(std::type_index type)
{
    static std::map<std::type_index, std::function<std::shared_ptr<IType>()>> typeMap{
        { typeid(std::string), String },
        { typeid(bool), Boolean },
        { typeid(int), Number },
        { typeid(unsigned int), Number },
        { typeid(float), Number },
        { typeid(double), Number },
        { typeid(std::byte), Number },
        { typeid(char), Number },
        { typeid(unsigned char), Number },
        { typeid(short), Number },
        { typeid(unsigned short), Number },
        { typeid(long), Number },
        { typeid(unsigned long), Number },
        { typeid(int8_t), Number },
        { typeid(uint8_t), Number },
        { typeid(int16_t), Number },
        { typeid(uint16_t), Number },
        { typeid(int32_t), Number },
        { typeid(uint32_t), Number },
        { typeid(int64_t), Number },
        { typeid(uint64_t), Number },
    };

    std::function typeFunc = Any;
    if(typeMap.find(type) != typeMap.end())
        typeFunc = typeMap[type];

    return typeFunc();
}
