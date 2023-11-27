extern "C" void xamarin_create_classes_Xamarin_Mac();

static void xamarin_invoke_registration_methods ()
{
	xamarin_create_classes_Xamarin_Mac();
}

#define MONOMAC 1
#include <xamarin/xamarin.h>
extern "C" void xamarin_create_classes_Xamarin_Mac ();



extern "C" int xammac_setup ()
{
	extern NSString* xamarin_custom_bundle_name;
	xamarin_custom_bundle_name = @"MonoBundle";
	xamarin_executable_name = "Bloxboot.exe";
	xamarin_marshal_objectivec_exception_mode = MarshalObjectiveCExceptionModeThrowManagedException;
	xamarin_disable_omit_fp = true;

	xamarin_debug_mode = TRUE;
	setenv ("MONO_GC_PARAMS", "major=marksweep", 1);
	xamarin_supports_dynamic_registration = TRUE;
	xamarin_mac_modern = TRUE;
	xamarin_invoke_registration_methods ();
	return 0;
}

