using HarmonyLib;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace RealCurrency;

public class CollectibleBehaviorStackShape : CollectibleBehavior, IContainedMeshSource
{
    public CollectibleBehaviorStackShape(CollectibleObject collObj) : base(collObj){}

    public override void GetHeldItemName(StringBuilder sb, ItemStack itemStack)
    {
        sb.Append($" ({Lang.Get($"{ModID}:{Currency}", itemStack.StackSize)})");
    }

    public override void OnBeforeRender(ICoreClientAPI capi, ItemStack itemstack, EnumItemRenderTarget target, ref ItemRenderInfo renderinfo)
    {
        Dictionary<string, MultiTextureMeshRef> meshes = ObjectCacheUtil.GetOrCreate(capi, "stacksizeMeshes", () => new Dictionary<string, MultiTextureMeshRef>());

        string key = GetMeshCacheKey(itemstack);

        //MeshData mesh = GenMesh(itemstack, null, null);
        //renderinfo.ModelRef = capi.Render.UploadMultiTextureMesh(mesh);

        if (!meshes.TryGetValue(key, out MultiTextureMeshRef meshref))
        {
            MeshData mesh = GenMesh(itemstack, null, null);
            meshref = meshes[key] = capi.Render.UploadMultiTextureMesh(mesh);
        }
        renderinfo.ModelRef = meshref;

        ModelTransform transform = Transformations.GetValueSafe(target);
        if (transform != null)
        {
            renderinfo.Transform = transform;
        }

        base.OnBeforeRender(capi, itemstack, target, ref renderinfo);
    }

    public MeshData GenMesh(ItemStack itemstack, ITextureAtlasAPI targetAtlas, BlockPos atBlockPos)
    {
        MeshData mesh = new MeshData(4, 3);

        float rotation = 0;
        float height = 0;

        int stacksize = itemstack.StackSize;
        int i = 0;
        while (stacksize > 0)
        {
            int count = stacksize / CurrencyDenominations[i];
            for (int j = 0; j < count; j++)
            {
                mesh.AddMeshData(GenBanknoteMesh(CurrencyDenominations[i].ToString(), ref rotation, ref height), 0, 0, height);
            }
            stacksize %= CurrencyDenominations[i];
            i++;
        }

        return mesh;
    }

    private MeshData GenBanknoteMesh(string denomination, ref float rotation, ref float height)
    {
        ICoreClientAPI capi = collObj.GetField<ICoreAPI>("api") as ICoreClientAPI;
        MeshData mesh = new MeshData(4, 3);

        height += 0.00625f;
        rotation -= 25;

        Dictionary<string, CompositeTexture> customTextures = new Dictionary<string, CompositeTexture>()
        {
            ["front"] = new CompositeTexture(AssetLocation.Create($"{ModID}:item/{Currency}/{denomination}-front")),
            ["rear"] = new CompositeTexture(AssetLocation.Create($"{ModID}:item/{Currency}/{denomination}-rear")),
            ["sides"] = new CompositeTexture(AssetLocation.Create("block/cloth/linen/plain")),
        };

        CompositeShape rcshape = new CompositeShape() { Base = AssetLocation.Create($"{ModID}:shapes/item/banknote.json") };
        if (rcshape == null)
        {
            return mesh;
        }

        Shape shape = capi.Assets.TryGet(rcshape.Base)?.ToObject<Shape>();
        if (shape == null)
        {
            return mesh;
        }

        ShapeTextureSource texSource = new ShapeTextureSource(capi, shape, "");
        foreach ((string textureCode, CompositeTexture texture) in customTextures)
        {
            CompositeTexture ctex = texture.Clone();
            ctex.Bake(capi.Assets);
            texSource.textures[textureCode] = ctex;
        }

        capi.Tesselator.TesselateShape("stacksize", shape, out mesh, texSource, new Vec3f(0, 0, rotation));
        return mesh;
    }

    public string GetMeshCacheKey(ItemStack itemstack)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(ModID);
        sb.Append("-");
        sb.Append(Core.ConfigRealCurrency.Currency);
        sb.Append("-");
        sb.Append(itemstack.StackSize);
        sb.Append("-");
        sb.Append(collObj.ToString());
        return sb.ToString();
    }
}
