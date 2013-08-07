#pragma once

namespace WindowsRuntimeComponent1
{
	namespace DX
	{
		inline void ThrowIfFailed(HRESULT hr)
		{
			if (FAILED(hr))
			{
				// Set a breakpoint on this line to catch DirectX API errors
				throw Platform::Exception::CreateException(hr);
			}
		}
	}

	public ref class D2DWraper sealed
	{
	public:
		Windows::Storage::Streams::InMemoryRandomAccessStream^  DrawTextToImage(Platform::String ^text,Platform::String ^srcFile, double left, double top, Platform::Boolean isHyperLink);
		//virtual void Render() = 0;
		//virtual void Present();
#if 1
		void SaveBitmapToFile();

		// Save render target bitmap to a stream using WIC.

#endif

		void Initialize(Windows::UI::Core::CoreWindow^ window);


	private:
		void CreateDeviceIndependentResources();
		void CreateDeviceResources();
		void CreateWindowSizeDependentResources();
		void UpdateForWindowSizeChange();
		void SetDpi(float dpi);
		void SaveBitmapToStream(
			Microsoft::WRL::ComPtr<ID2D1Bitmap1> d2dBitmap,
			Microsoft::WRL::ComPtr<IWICImagingFactory2> wicFactory2,
			Microsoft::WRL::ComPtr<ID2D1DeviceContext> d2dContext,
			REFGUID wicFormat,
			IStream* stream
			);

	protected private:
		// Direct2D Objects
		Microsoft::WRL::ComPtr<ID2D1Factory1>          m_d2dFactory;
		Microsoft::WRL::ComPtr<ID2D1Device>            m_d2dDevice;
		Microsoft::WRL::ComPtr<ID2D1DeviceContext>     m_d2dContext;
		Microsoft::WRL::ComPtr<ID2D1Bitmap1>           m_d2dTargetBitmap;



		// DirectWrite & Windows Imaging Component Objects
		Microsoft::WRL::ComPtr<IDWriteFactory1>        m_dwriteFactory;
		Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>    m_whiteBrush;
		Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>	m_blackBrush;
		Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>	m_blueBrush;
		Microsoft::WRL::ComPtr<ID2D1DrawingStateBlock>  m_stateBlock;
		Microsoft::WRL::ComPtr<IWICImagingFactory2>      m_wicFactory;
		Microsoft::WRL::ComPtr<ID2D1Bitmap1>             m_logoBitmap;
		Microsoft::WRL::ComPtr<IDWriteTextLayout>       m_textLayout;
		// Direct3D Objects
		Microsoft::WRL::ComPtr<ID3D11Device1>          m_d3dDevice;
		Microsoft::WRL::ComPtr<ID3D11DeviceContext1>   m_d3dContext;
		Microsoft::WRL::ComPtr<IDXGISwapChain1>        m_swapChain;
		Microsoft::WRL::ComPtr<ID3D11RenderTargetView> m_renderTargetView;
		Microsoft::WRL::ComPtr<ID3D11DepthStencilView> m_depthStencilView;

		D3D_FEATURE_LEVEL                              m_featureLevel;
		Windows::Foundation::Size                      m_renderTargetSize;
		Windows::Foundation::Rect                      m_windowBounds;
		Windows::UI::Core::CoreWindow^                 m_window;
		float                                          m_dpi;

		Microsoft::WRL::ComPtr<IDWriteTextFormat> nameTextFormat;
		Platform::String^                                               m_imageFileName;
		Windows::Storage::Streams::InMemoryRandomAccessStream^ m_inMemStream;
		Platform::Array<byte>^ m_larrBuffer;
	};
}