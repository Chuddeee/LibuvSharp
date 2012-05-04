using System;
using System.Runtime.InteropServices;

namespace LibuvSharp
{
	public class DynamicLibrary
	{
		[DllImport("uv", CallingConvention = CallingConvention.Cdecl)]
		internal extern static uv_err_t uv_dlopen(IntPtr name, out IntPtr handle);

		[DllImport("uv", CallingConvention = CallingConvention.Cdecl)]
		internal extern static uv_err_t uv_dlopen(string name, out IntPtr handle);

		[DllImport("uv", CallingConvention = CallingConvention.Cdecl)]
		internal extern static uv_err_t uv_dlclose(IntPtr handle);

		[DllImport("uv", CallingConvention = CallingConvention.Cdecl)]
		internal extern static uv_err_t uv_dlsym(IntPtr handle, string name, out IntPtr ptr);

		IntPtr handle;

		public bool Closed {
			get {
				return handle == IntPtr.Zero;
			}
		}

		public DynamicLibrary()
		{
			Ensure.Success(uv_dlopen(IntPtr.Zero, out handle));
		}

		public DynamicLibrary(string library)
		{
			Ensure.ArgumentNotNull(library, "library");

			var error = uv_dlopen(library, out handle);
			if (error.code != uv_err_code.UV_OK) {
				throw new Exception();
			}
		}

		public void Close()
		{
			if (!Closed) {
				uv_dlclose(handle);
				handle = IntPtr.Zero;
			}
		}

		public bool TryGetSymbol(string name, out IntPtr pointer)
		{
			return uv_dlsym(handle, name, out pointer).code == uv_err_code.UV_OK;
		}

		public IntPtr GetSymbol(string name)
		{
			IntPtr ptr;
			Ensure.Success(uv_dlsym(handle, name, out ptr));
			return ptr;
		}
	}
}

