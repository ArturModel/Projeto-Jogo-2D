"""Pack 32 newly drawn ImageGen poses; no synthesized animation or pose warping."""
from pathlib import Path
from PIL import Image
import json,sys

source=Path(sys.argv[1]); destination=Path(sys.argv[2])
destination.mkdir(parents=True,exist_ok=True)
image=Image.open(source).convert('RGBA')
assert image.getchannel('A').getextrema()[0]==0, 'Real transparent background required'
cells=[]
for row in range(4):
    for col in range(8):
        cell=image.crop((round(col*image.width/8),round(row*image.height/4),round((col+1)*image.width/8),round((row+1)*image.height/4)))
        bounds=cell.getchannel('A').point(lambda a:255 if a>127 else 0).getbbox()
        assert bounds, (row,col)
        assert bounds[0]>0 and bounds[1]>0 and bounds[2]<cell.width and bounds[3]<cell.height,(row,col,'source clipped',bounds)
        cells.append((cell,bounds))
# One global scale retains anatomy and relative height; feet sit on y=115.
scale=100/max(b[3]-b[1] for _,b in cells)
atlas=Image.new('RGBA',(1024,512))
report=[]
for index,(cell,bounds) in enumerate(cells):
    x0,y0,x1,y1=bounds
    crop=cell.crop(bounds).resize((round((x1-x0)*scale),round((y1-y0)*scale)),Image.Resampling.NEAREST)
    x=round(64+(x0-cell.width/2)*scale);y=115-crop.height
    assert x>=2 and x+crop.width<127 and y>=2,(index,x,y)
    atlas.alpha_composite(crop,(index%8*128+x,index//8*128+y))
    report.append(dict(cell=index,row=index//8,pose=index%8,bounds=[x,y,x+crop.width,115]))
atlas.save(destination/'FrequentadoresSinuca.png')
(destination/'cells.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(f'PASS: {len(cells)} distinct poses, common scale {scale:.6f}, 128px cells, feet y=115, real alpha, no clipping')
