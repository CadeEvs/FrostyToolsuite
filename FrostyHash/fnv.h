
#pragma once

#include <stddef.h>
#include <stdint.h>

uint32_t FNV1_32_Hash(const char* data, int len);
uint32_t FNV1a_32_Hash(const char* data, int len);
uint64_t FNV1_64_Hash(const char* data, int len);
uint64_t FNV1a_64_Hash(const char* data, int len);