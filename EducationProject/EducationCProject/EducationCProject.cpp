#include <iostream> // Для вывода на консоль
#include <algorithm>
#include <list>
#include <string>

#define SQ(X) X*X


using namespace std;
/////////////////////////////////////////////////////////////////////////////////////////
//class A {
//public:
//	A() :x(0), y(0), z(0) {}
//	A(int a, int b) : z(y + 1), y(2 * x + b), x(a) { cout << z << endl; }
//private:
//	int x;
//	int y;
//	int z;
//};
/////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////
//int x = 5;
//void f(int var)
//{
//int y = var;
//int x = ::x;
//while (x < 20) {
//	int x = 2;
//	y += x;
//	if (y > 10) break;
//	cout << y;
//	x += 10;
//}
//cout << x << endl;
//}
/////////////////////////////////////////////////////////////////////////////////////////

using ullong = unsigned long long; // или typedef unsigned long long ullong;

int main()
{
//	std::cout << "Привет мир!" << std::endl;	// Так русский не понимает....
//
//	setlocale(LC_ALL, "");
//	std::wcout << L"Привет мир!" << std::endl;	// Для вывода русского текста вот такая фигня нужна
//
//	int age;
//	int age1(10);
//	int age2{ 30 };
//	age = 28;
//	std::cout << "Age = " << age << endl;
//	std::cout << "Age1 = " << age1 << endl;
//	std::cout << "Age2 = " << age2 << endl;
//
//	char a1{ 'A' };
//	char a2{ 65 };
//	std::cout << "a1: " << a1 << std::endl;
//	std::cout << "a2: " << a2 << std::endl;
//
//	wchar_t a3{ L'A' };
//	wchar_t a4{ L'\x41' };
//	std::wcout << a3 << '\n';
//	std::wcout << a4 << '\n';
//
//	auto number = 5;        // number имеет тип int
//	auto sum{ 1234.56 };    // sum имеет тип double
//	auto distance{ 267UL };  // distance имеет тип unsigned long
//
//	//int age5;
//	//double weight;
//	//std::cout << "Input age: ";
//	//std::cin >> age5;
//	//std::cout << "Input weight: ";
//	//std::cin >> weight;
//	//std::cout << "Age: " << age5 << "\t Weight: " << weight << std::endl;
//
//	int a{ 26 };
//	int b{ 5 };
//	int c{ a % b };      // c = 26 % 5 = 26 - 5 * 5 = 1
//	int d{ 4 % b };     // d = 4 % 5 = 4
//
//	ullong n{ 10234 };
//	std::cout << n << std::endl;
//
//
//	//float num1{ 1.23E-4 };        // 0.000123
//	//float num2{ 3.65E+6 };        // 3650000
//	//float sum{ num1 + num2 };       // sum =3.65e+06
//	//std::cout << "sum =" << sum << "\n";
//
//	//int km{ 0 };
//	//int m1{ 0 };
//	//int m{ 0 };
//	//std::cout << "Input m: ";
//	//std::cin >> m1;
//	//m = m1 % 1000;
//	//km = m1 / 1000;
//
//	//setlocale(LC_ALL, "");
//	//std::cout << km;
//	//std::wcout << L" километра, " << endl;
//	//std::cout << m;
//	//std::wcout << L" метров" << endl;
//
//	int n1{ 5 };
//	unsigned int x{ 8 };
//	std::cout << "result = " << n1 - x << std::endl;   // result = 4294967293
//
//	double sum1{ 100.2 };
//	unsigned int hours{ 8 };
//	unsigned int revenuePerHour{ static_cast<unsigned int>(sum1 / hours) };  // revenuePerHour = 12. Прведение типов по новому вместо (unsigned int)
//	std::cout << "Revenue per hour = " << revenuePerHour << std::endl;
//
//	////////////////////// Кодировка 3-х двухбитовых чисел в одно////////////////////////////////////
//	int value1{ 3 };  // 0b0000'0011
//	int value2{ 2 };  // 0b0000'0010
//	int value3{ 1 };  // 0b0000'0001
//	int result{ 0b0000'0000 };
//	// сохраняем в result значения из value1
//	result = result | value1; // 0b0000'0011
//	// сдвигаем разряды в result на 2 разряда влево
//	result = result << 2;   // 0b0000'1100
//	// сохраняем в result значения из value2
//	result = result | value2;  // 0b0000'1110
//	// сдвигаем разряды в result на 2 разряда влево
//	result = result << 2;   // 0b0011'1000
//	// сохраняем в result значения из value3
//	result = result | value3;  // 0b0011'1001
//	std::cout << result << std::endl;   // 57
//
//	/*или так:
//	int value1 {3};  // 0b0000'0011
//    int value2 {2};  // 0b0000'0010
//    int value3 {1};  // 0b0000'0001
//    int result {0b0000'0000};
//    // сохраняем в result значения из value1
//    result = result | (value1 << 4);
//    // сохраняем в result значения из value2
//    result = result | (value2 << 2);
//    // сохраняем в result значения из value3
//    result = result | value3;  // 0b0011'1001
//    std::cout << result << std::endl;   // 57
//	*/
//
//	/////////////////////////////Расшифровка /////////////////////////////////////////
//	int result1{ 0b0011'1001 };
//	// обратное получение данных
//	int newValue3 = result & 0b000'0011;
//	// сдвигаем данные на 2 разряда вправо
//	result1 = result1 >> 2;
//	int newValue2 = result1 & 0b000'0011;
//	// сдвигаем данные на 2 разряда вправо
//	result1 = result1 >> 2;
//	int newValue1 = result1 & 0b000'0011;
//	std::cout << newValue1 << std::endl;   // 3
//	std::cout << newValue2 << std::endl;   // 2
//	std::cout << newValue3 << std::endl;   // 1
//	////////////////////////////////////////////////////////////////////////////////////////////////
//
//	//unsigned char Sym1{ 0 }, Sym2{ 0 }, Sym3{ 0 }, Sym4{ 0 };
//	//unsigned int SymSum{ 0 };
//	//unsigned char Mask{ 255 };
//	//unsigned char Res1{ 0 }, Res2{ 0 }, Res3{ 0 }, Res4{ 0 };
//
//	//std::cout << "Input one: ";
//	//std::cin >> Sym1;
//	//std::cout << "Input two: ";
//	//std::cin >> Sym2;
//	//std::cout << "Input three: ";
//	//std::cin >> Sym3;
//	//std::cout << "Input four: ";
//	//std::cin >> Sym4;
//
//	//SymSum |= (Sym1 << 24);
//	//SymSum |= (Sym2 << 16);
//	//SymSum |= (Sym3 << 8);
//	//SymSum |= Sym4;
//
//	//Res4 = SymSum & Mask;
//	//Res3 = (SymSum >> 8) & Mask;
//	//Res2 = (SymSum >> 16) & Mask;
//	//Res1 = (SymSum >> 24) & Mask;
//
//	//std::cout << "Sum = " << SymSum << std::endl;   // 3
//	//std::cout << "Res1 = " << Res1;   // 3
//	//std::cout << ", Res2 = " << Res2;   // 2
//	//std::cout << ", Res3 = " << Res3;   // 1
//	//std::cout << ", Res4 = " << Res4 << std::endl;   // 1
//
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////
//	int aa{ 5 };
//	int bb{ 8 };
//	bool result11 = aa == 5 && bb > 8;  // если и a ==5, и b > 8
//	bool result22 = aa == 5 || bb > 8;  // если или a ==5, или b > 8 (или оба варианты истины)
//	bool result33 = aa == 5 ^ bb > 8;  // если оба операнда возвращают разные значения
//
//	std::cout << "(a ==5 && b > 8) - " << std::boolalpha << result11 << std::endl;
//	std::cout << "(a ==5 || b > 8) - " << std::boolalpha << result22 << std::endl;
//	std::cout << "(a ==5 ^ b > 8) - " << std::boolalpha << result33 << std::endl;
//
//
//	std::cout << (aa < bb ? "a is less than b" :
//		(aa == bb ? "a is equal to b" : "a is greater than b"));
//
//	int numbers[]{ 1, 2, 3, 4, 5 };
//	// n - константная ссылка
//	for (const auto& n : numbers)
//	{
//		std::cout << n << "\t";
//	}
//	std::cout << std::endl;
//
//	int count = std::size(numbers);	//	Длина массива
//	std::cout << "Length: " << count << std::endl;   // Length: 4
//
//	for (int n : numbers)
//	{
//		std::cout << n << std::endl; // Перебор массива
//	}
//
//	int xx = 5;
//	double aaa = SQ(xx + 1);
//	cout << aaa << endl;
//
//
//	///////////////Про многомерные массивы ////////////////////////////////////////////////
//
//	const int rows = 3, columns = 2;
//	int numbers1[rows][columns]{ {1, 2}, {3, 4}, {5, 6} };
//
//	for (auto& Subnumbers1 : numbers1)
//	{
//		for (int number : Subnumbers1)
//		{
//			std::cout << number << "\t";
//		}
//		std::cout << std::endl;
//	}
//
//	///////////////////////////////////////////////////////////////////////////////////////
//	//f(5);
//	///////////////////////////////////////////////////////////////////////////////////////
//
//	///////////////////////////////////////////////////////////////////////////////////////
//	//A(2, 3);
//	///////////////////////////////////////////////////////////////////////////////////////
//
//	///////////////////////////////////////////////////////////////////////////////////////
//	//list<int> l = { 0,1,2,3,4,5,6 };
//	//auto it = remove_if(l.begin(), l.end(), [](const int x)->bool
//	//	{
//	//		return x % 3 != 0;
//	//	});
//	//l.erase(it, l.end());
//	//cout << l.size();
//	///////////////////////////////////////////////////////////////////////////////////////

//std::string name;
//std::cout << "Input your name: ";
//getline(std::cin, name);
//std::cout << "Your name: " << name << std::endl;


//const int MaxLength{ 10 };
//char StringInput[MaxLength]{};
//std::cin.getline(StringInput, MaxLength, '\n');
//int CountSymbol{ 0 };
//
//for(char& Ch:StringInput)
//{ 
//	if(Ch=='\0') 
//	{
//		break;
//	}
//	else
//	{
//	CountSymbol++;
//	}
//		
//}
//
//for (int i{ CountSymbol }; i >= 0 ; i--)
//{
//	std::cout << StringInput[i];
//}
//
//std::cout << endl;
//
//std::cout << CountSymbol << endl;

//int a{ 10 };
//int b{ 6 };
//
//int* p{};           // указатель
//int*& pRef{ p };     // ссылка на указатель
//pRef = &a;          // через ссылку указателю p присваивается адрес переменной a
//std::cout << "p value=" << *p << std::endl;   // 10
//*pRef = 70;         // изменяем значение по адресу, на который указывает указатель
//std::cout << "a value=" << a << std::endl;    // 70
//
//pRef = &b;          // изменяем адрес, на который указывает указатель
//std::cout << "p value=" << *p << std::endl;   // 6

    //const char* Langs[]{ "C++", "Python", "JavaScript" }; // Массив указателей
    //// перебор массива
    //for (unsigned i{}; i < std::size(Langs); i++)
    //{
    //    std::cout << Langs[i] << std::endl;
    //}
const int Col{ 20 };
int Numbers[Col] {};
int Counter{ 1 };

	for (auto& SubNumbers : Numbers)
	{
        SubNumbers = Counter;
        Counter= Counter+2;
	}

    for (unsigned i{0}; i < Col; i++)
    {
        if (((i+1) % 5) == 0) 
        {
            std::cout << *(Numbers + i) << std::endl;
        }
        else
        {
         std::cout << *(Numbers+i)<< "\t";
        }
    }

    std::cout << ' ' << std::endl;

    for (unsigned i{ 0 }; i < Col; i++)
    {
        if (((i + 1) % 5) == 0)
        {
            std::cout << *(Numbers + Col - i-1) << std::endl;
        }
        else
        {
            std::cout << *(Numbers +Col - i-1) << "\t";
        }
    }

	return 0;
}