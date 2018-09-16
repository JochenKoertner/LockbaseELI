// LbwELI Demo Shared Library template
#include <string>

// Callback function type
typedef int (*ELIDrv2AppCallback)( const char* sSessID, int nJob, const char* sJob/*=NULL*/ );
// Application callback function pointer, NULL until initialized
static ELIDrv2AppCallback ELIDrv2App = NULL;

// In case a driver function returns a const char*, it will point to the contents of this string.
// It will remain valid until the next invocation of a driver function (which may also return text).
static std::string sDrvReturn;

// Some compiler specific stuff (to be extended)
#ifdef _WIN32
#define DLLEXPORT extern "C" __declspec(dllexport)
#else
#define DLLEXPORT extern "C"
#endif 

DLLEXPORT const char* ELICreate( const char* sLic, const char* sLbwELIRev, ELIDrv2AppCallback Drv2App )
// Constructor, the application identifies by licence number and revision
// and provides a callback function
{
	// Install callback function
	ELIDrv2App = Drv2App;
	// Check revision
	//	...
	// Done
	sDrvReturn = "EOK";
	return sDrvReturn.c_str();
}

DLLEXPORT void ELIDestroy()
// Interface destructor, the application detaches from the driver
{
	// Clean up
}

DLLEXPORT const char* ELIDriverInfo()
// Application requests driver global information, e.g. revision and supported products
{
	// Collect the driver info in sDrvReturn
	// ...
	return sDrvReturn.c_str();
}

DLLEXPORT void ELIDriverUI( const char* SessID, const char* SID )
// Application launches driver's user interface on user command, e.g. a driver provided
// dialogue box, etc.
{
	// Show our extra Setup dialogue
	// ...
}

DLLEXPORT const char* ELIProductInfo( const char* sProductID )
// Application requests information about capabilities and limitations of a certain product, 
// e.g. programming capacity or event types
{
	// Collect the product info in sDrvReturn
	// ...
	return sDrvReturn.c_str();
}

DLLEXPORT const char* ELISystemInfo( const char* sUsers )
// Application requests information about available systems depending on licence, revision and users
{
	// Collect system information in sDrvReturn
	// ...
	return sDrvReturn.c_str();
}

DLLEXPORT const char* ELIOpen( const char* sUserList, const char* sSystem/*=NULL*/, const char* sExtData/*=NULL*/ )
// Application logs into a system and receives a session id
{
	// Write return text to sDrvReturn
	// ...
	return sDrvReturn.c_str();
}

DLLEXPORT const char* ELIClose( const char* sSessID )
// Application closes a given session (pending jobs will still be processed)
{
	// Write return code and extra data to sDrvReturn
	// ...
	return sDrvReturn.c_str();
}

DLLEXPORT int ELIApp2Drv( const char* sSessID, int nJob, const char* sJob/*=NULL*/ )
// Application launches a job in a given session
{
	// Process job
	// ...
	// Return result via ELIDrv2App
	// This is just to demonstrate the callback. Unsually you will NOT do this here, because job processing will be asynchonous.
	if( ELIApp2Drv ) {
		// Job data have been collected in sDrvReturn
		return ELIApp2Drv( sSessID, nJob, sDrvReturn.c_str() );
	}
	// Something went wrong
	return -1;
}

