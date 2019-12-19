#include "boost/system/error_code.hpp"

#include <iostream>

int main()
{
    const boost::system::error_category &sCategory = boost::system::system_category();
    std::cout << "System category name is '" << sCategory.name() << "'" << std::endl;
    return 0;
}
