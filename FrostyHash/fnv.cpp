
#include "stdafx.h"
#include "fnv.h"

uint32_t FNV1_32_Hash(const char* data, int len)
{
	static const uint32_t kOffset = 5381;
	static const uint32_t kPrime = 33;

	const uint8_t* octets = reinterpret_cast<const uint8_t*>(data);
	uint32_t hash = kOffset;

	for (int i = 0; i < len; ++i)
	{
		hash = (hash * kPrime) ^ octets[i];
	}

	return hash;
}

uint32_t FNV1a_32_Hash(const char* data, int len)
{
	static const uint32_t kOffset = 5381;
	static const uint32_t kPrime = 33;

	const uint8_t* octets = reinterpret_cast<const uint8_t*>(data);
	uint32_t hash = kOffset;

	for (int i = 0; i < len; ++i)
	{
		hash = hash ^ octets[i];
		hash = hash * kPrime;
	}

	return hash;
}

uint64_t FNV1_64_Hash(const char* data, int len)
{
	static const uint64_t kOffset = UINT64_C(14695981039346656037);
	static const uint64_t kPrime = UINT64_C(1099511628211);

	const uint8_t* octets = reinterpret_cast<const uint8_t*>(data);

	uint64_t hash = kOffset;

	for (int i = 0; i < len; ++i)
	{
		hash = (hash * kPrime) ^ octets[i];
	}

	return hash;
}

uint64_t FNV1a_64_Hash(const char* data, int len)
{
	static const uint64_t kOffset = UINT64_C(14695981039346656037);
	static const uint64_t kPrime = UINT64_C(1099511628211);

	const uint8_t* octets = reinterpret_cast<const uint8_t*>(data);

	uint64_t hash = kOffset;

	for (int i = 0; i < len; ++i) 
	{
		hash = hash ^ octets[i];
		hash = hash * kPrime;
	}

	return hash;
}
