"""Pack the original sixteen drawings and eight ImageGen directional shot drawings.
No pose synthesis. Original source and pixel alignment are preserved.
"""
from pathlib import Path
from PIL import Image
root=Path(__file__).resolve().parents[2]
assets=root/'Projeto Jogo 2D/Assets'
art=root/'Arte/Producao/ZeSinuca'
generated=art/'ZeSinuca_FrenteCostas_Gerado.png'
# Packing only: preserve the original sixteen drawings pixel for pixel.
old=Image.open(art/'ZeSinuca_Atlas.png').convert('RGBA')
atlas=Image.new('RGBA',(512,768));atlas.alpha_composite(old,(0,0))
im=Image.open(generated).convert('RGBA')
cells=[]
for i in range(8):
    c,r=i%4,i//4
    cell=im.crop((round(c*im.width/4),round(r*im.height/2),round((c+1)*im.width/4),round((r+1)*im.height/2)))
    bounds=cell.getchannel('A').point(lambda a:255 if a>127 else 0).getbbox()
    if not bounds:raise ValueError(i)
    cells.append((cell,bounds))
scale=94/max(cells[i][1][3]-cells[i][1][1] for i in (0,4))
for i,(cell,bounds) in enumerate(cells):
    x0,y0,x1,y1=bounds
    cut=cell.crop(bounds).resize((round((x1-x0)*scale),round((y1-y0)*scale)),Image.Resampling.NEAREST)
    x=round(64+(x0-cell.width/2)*scale);y=113-cut.height
    assert x>0 and x+cut.width<128 and y>0,(i,x,y)
    atlas.alpha_composite(cut,(i%4*128+x,(4+i//4)*128+y))
atlas.save(assets/'Resources/Art/Characters/Npc_Ze_Sinuca.png')
atlas.save(art/'ZeSinuca_Direcional_Atlas.png')

print('Exported 24 directional Ze frames, 512x768, original sixteen unchanged.')
