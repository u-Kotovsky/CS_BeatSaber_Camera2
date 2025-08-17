using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using RTID = UnityEngine.Rendering.RenderTargetIdentifier;

namespace Klak.Spout {

static class RendererOverride
{
    static MaterialPropertyBlock _block;

    public static void SetTexture
      (Renderer renderer, string property, Texture texture)
    {
        if (_block == null) _block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(_block);
        _block.SetTexture(property, texture);
        renderer.SetPropertyBlock(_block);
    }
}

static class Blitter
{
		private static AssetBundle _bundle;
		private static Shader _shader;

    public static void Blit
      (SpoutResources resrc, Texture src, RenderTexture dst, bool alpha)
      => Graphics.Blit(src, dst, GetMaterial(resrc), alpha ? 0 : 1);

    public static void BlitVFlip
      (SpoutResources resrc, Texture src, RenderTexture dst, bool alpha)
      => Graphics.Blit(src, dst, GetMaterial(resrc), alpha ? 2 : 3);

    public static void Blit
      (SpoutResources resrc, CommandBuffer cb, RTID src, RTID dst, bool alpha)
      => cb.Blit(src, dst, GetMaterial(resrc), alpha ? 0 : 1);

    public static void BlitFromSrgb
      (SpoutResources resrc, Texture src, RenderTexture dst)
      => Graphics.Blit(src, dst, GetMaterial(resrc), 4);

    static Material _material;


		static Material GetMaterial(SpoutResources resrc) {
			if(_material == null) {
				Debug.LogError("spout blit shader is null");
				try {
					// try to load asset bundle
					if(_bundle == null) {
						_bundle = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath, "AssetBundles/spout"));
						Debug.Log("Loaded spout bundle.");
					}
					if(_shader == null) {
						_shader = _bundle.LoadAsset<Shader>("Assets/spout/Shaders/Blit.shader");
						Debug.Log("Loaded spout blit shader asset.");
					}
					/*var res = ScriptableObject.CreateInstance<SpoutResources>();
					res.blitShader = _shader;
					_resources = res;*/
					Debug.Log($"blit shader isnull?: {_shader == null}");
				} catch(Exception e) {
					Debug.LogError("shader are null in Spout.");
					throw e;
				}

				_material = new Material(_shader);
				_material.hideFlags = HideFlags.DontSave;
			}
			return _material;
		}
	}


	static class Utility
{
    public static void Destroy(UnityEngine.Object obj)
    {
        if (obj == null) return;

        if (Application.isPlaying)
				UnityEngine.Object.Destroy(obj);
        else
				UnityEngine.Object.DestroyImmediate(obj);
    }
}

} // namespace Klak.Spout
