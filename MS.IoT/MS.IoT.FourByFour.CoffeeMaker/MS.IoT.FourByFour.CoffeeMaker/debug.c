#include <stdio.h>
#include <stdlib.h>
#include <applibs/log.h>

typedef struct {
	unsigned long size, resident, share, text, lib, data, dt;
} statm_t;

void read_off_memory_status()
{
	statm_t result;
	const char* statm_path = "/proc/self/statm";

	FILE *f = fopen(statm_path, "r");
	if (!f) {
		perror(statm_path);
		abort();
	}
	if (7 != fscanf(f, "%ld %ld %ld %ld %ld %ld %ld",
		&result.size, &result.resident, &result.share, &result.text, &result.lib, &result.data, &result.dt))
	{
		perror(statm_path);
		abort();
	}
	Log_Debug("%ld %ld %ld %ld %ld %ld %ld\n", result.size, result.resident, result.share, result.text, result.lib, result.data, result.dt);
	fclose(f);
}