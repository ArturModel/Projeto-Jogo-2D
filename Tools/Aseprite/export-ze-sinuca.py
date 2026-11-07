"""Normalize the newly drawn atlas into 16 aligned cells. Never synthesize animation poses."""
from pathlib import Path
from PIL import Image
import shutil, uuid

root=Path(__file__).resolve().parents[2]
art=root/'Arte/Producao/ZeSinuca'
source=art/'ZeSinuca_Gerado.png'
dest=root/'Projeto Jogo 2D/Assets/Resources/Art/Characters/Npc_Ze_Sinuca.png'
im=Image.open(source).convert('RGBA')
atlas=Image.new('RGBA',(512,512))
for i in range(16):
    # The generated source is 1254 square; rounded boundaries avoid cumulative drift.
    col,row=i%4,i//4
    cell=im.crop((round(col*im.width/4),round(row*im.height/4),round((col+1)*im.width/4),round((row+1)*im.height/4)))
    bounds=cell.getchannel('A').point(lambda a:255 if a>127 else 0).getbbox()
    if bounds is None: raise ValueError(f'Empty animation cell {i}')
    x0,y0,x1,y1=bounds
    # Fixed uniform scale and original cell centre keep dimensions and limb poses intact.
    scale=.38
    cropped=cell.crop(bounds)
    cropped=cropped.resize((round(cropped.width*scale),round(cropped.height*scale)),Image.Resampling.NEAREST)
    x=round(64+(x0-cell.width/2)*scale)
    y=113-cropped.height
    if x<0 or x+cropped.width>128 or y<0: raise ValueError(f'Clipped animation cell {i}')
    atlas.alpha_composite(cropped,(col*128+x,row*128+y))
atlas.save(dest)
atlas.save(art/'ZeSinuca_Atlas.png')
meta=Path(str(dest)+'.meta')
if not meta.exists():
    template=dest.with_name('Npc_Ze.png.meta').read_text(encoding='utf-8-sig')
    import re
    template=re.sub(r'guid: [a-f0-9]+','guid: '+uuid.uuid4().hex,template,count=1)
    template=template.replace('Npc_Ze_0','Npc_Ze_Sinuca_0')
    meta.write_text(template,encoding='utf-8')
print('Exported 16 newly drawn poses, 512x512 RGBA, 128px cells, shared floor at y=113; no pose deformation.')

# Include the supplementary directional shots when their source is available.
if (art/"ZeSinuca_FrenteCostas_Gerado.png").exists():
    import runpy
    runpy.run_path(str(Path(__file__).with_name("export-ze-directional.py")),run_name="__main__")
