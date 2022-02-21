
#pragma unmanaged
#include "stdafx.h"

#pragma managed
#include "FrostyHash.h"

#include "city.h"
#include "MurmurHash2.h"
#include "fnv.h"

namespace Frosty::Hash
{
	//
	// FNV1
	//

	int Fnv1::Hash(array<Byte>^ data)
	{
		if (data->Length == 0)
			return 5381;
		pin_ptr<Byte> ptr = &data[0];
		return (int)FNV1_32_Hash((char*)ptr, data->Length);
	}

	int Fnv1::HashString(String^ data)
	{
		return Hash(System::Text::Encoding::UTF8->GetBytes(data));
	}

	unsigned long long Fnv1::Hash64(array<Byte>^ data)
	{
		if (data->Length == 0)
			return 0;
		pin_ptr<Byte> ptr = &data[0];
		return FNV1_64_Hash((char*)ptr, data->Length);
	}

	unsigned long long Fnv1::HashString64(String^ data)
	{
		return Hash64(System::Text::Encoding::UTF8->GetBytes(data));
	}

	//
	// FNV1a
	//

	int Fnv1a::Hash(array<Byte>^ data)
	{
		if (data->Length == 0)
			return 5381;
		pin_ptr<Byte> ptr = &data[0];
		return (int)FNV1a_32_Hash((char*)ptr, data->Length);
	}

	int Fnv1a::HashString(String^ data)
	{
		return Hash(System::Text::Encoding::UTF8->GetBytes(data));
	}

	unsigned long long Fnv1a::Hash64(array<Byte>^ data)
	{
		if (data->Length == 0)
			return 0;
		pin_ptr<Byte> ptr = &data[0];
		return FNV1a_64_Hash((char*)ptr, data->Length);
	}

	unsigned long long Fnv1a::HashString64(String^ data)
	{
		return Hash64(System::Text::Encoding::UTF8->GetBytes(data));
	}

	//
	// CityHash
	//

	unsigned long long CityHash::Hash64(array<Byte>^ data)
	{
		if (data->Length == 0)
			return 0;
		pin_ptr<Byte> ptr = &data[0];
		return CityHash64((const char*)ptr, data->Length);
	}

	/*unsigned long long CityHash::HashWithSeed64(array<Byte>^ data, unsigned long long seed)
	{
		if (data->Length == 0)
			return 0;
		pin_ptr<Byte> ptr = &data[0];
		return CityHash64WithSeed((const char*)ptr, data->Length, seed);
	}*/

	//
	// Murmur2
	//

	unsigned long long Murmur2::Hash64(array<Byte>^ data, unsigned long long seed)
	{
		if (data->Length == 0)
			return 0;
		pin_ptr<Byte> ptr = &data[0];
		return MurmurHash64A(ptr, data->Length, seed);
	}

	unsigned long long Murmur2::HashString64(String^ data, unsigned long long seed)
	{
		return Hash64(System::Text::Encoding::UTF8->GetBytes(data), seed);
	}
}