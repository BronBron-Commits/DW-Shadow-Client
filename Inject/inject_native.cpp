// 1. The full path to the DLL
const char_t* assembly_path = L"C:\\Path\\To\\U-DW-ManagedWrapper.dll";

// 2. The Type Name: "Namespace.Class, AssemblyName"
// DO NOT include .dll in the AssemblyName part here.
const char_t* type_name = L"DeltaWorlds.Injection.EntryPoint, U-DW-ManagedWrapper";

// 3. The Method Name
const char_t* method_name = L"Load";

// 4. The Delegate Type: NULL (UNMANAGED_CALLERS_ONLY_METHOD)
// If you pass a specific delegate name here, it implies a managed delegate, not UnmanagedCallersOnly.
// Keeping it NULL tells CoreCLR to look for [UnmanagedCallersOnly].
const char_t* delegate_type_name = nullptr; 

typedef int (CORECLR_DELEGATE_CALLTYPE *custom_entry_point_fn)(void* args, int size);
custom_entry_point_fn managed_load = nullptr;

int rc = load_assembly_and_get_function_pointer(
    assembly_path,
    type_name,
    method_name,
    delegate_type_name, // MUST BE NULL/nullptr
    nullptr,
    (void**)&managed_load
);

if (rc != 0 || managed_load == nullptr) {
    // Log failure rc
} else {
    // Execute
    int result = managed_load(nullptr, 0);
    // If result is 1337, you win.
}