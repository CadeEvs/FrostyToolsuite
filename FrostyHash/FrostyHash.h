#pragma once

using namespace System;

namespace Frosty::Hash 
{
	public ref class Fnv1
	{
	public:
		static int Hash(array<Byte>^ data);
		static int HashString(String^ data);

		static unsigned long long Hash64(array<Byte>^ data);
		static unsigned long long HashString64(String^ data);
	};

	public ref class Fnv1a
	{
	public:
		static int Hash(array<Byte>^ data);
		static int HashString(String^ data);

		static unsigned long long Hash64(array<Byte>^ data);
		static unsigned long long HashString64(String^ data);
	};

	public ref class CityHash
	{
	public:
		static unsigned long long Hash64(array<Byte>^ data);
		//static unsigned long long HashWithSeed64(array<Byte>^ data, unsigned long long seed);
	};

	public ref class Murmur2
	{
	public:
		static unsigned long long Hash64(array<Byte>^ data, unsigned long long seed);
		static unsigned long long HashString64(String^ data, unsigned long long seed);
	};
}
