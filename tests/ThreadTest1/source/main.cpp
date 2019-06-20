#include "boost/thread.hpp"

#include <iostream>

struct ThreadFunction
{
    void operator()()
    {
        std::cout << "Worker thread id is " << boost::this_thread::get_id() << std::endl;
        boost::this_thread::sleep_for(boost::chrono::seconds(10));
    }
};

int main()
{
    std::cout << "Main thread id is " << boost::this_thread::get_id() << std::endl;
    ThreadFunction callable;
    boost::thread thread1 = boost::thread(callable);
    boost::thread thread2 = boost::thread(callable);
    thread1.join();
    thread2.join();
    std::cout << "Threads have been joined" << std::endl;
    return 0;
}
