local root=assert(app.params.root)
local atlas=app.open(assert(app.params.source))
local source=atlas.cels[1].image
local names={"Tree_A","Tree_B","Araucaria","Shrub","Flowers","GrassClump","PicketFence","Fern"}
local heights={160,160,176,64,32,32,48,40}
for i,name in ipairs(names) do
  local cx,cy=(i-1)%4,math.floor((i-1)/4)
  -- Neste atlas as árvores ocupam a faixa superior maior; a referência visual
  -- foi inspecionada e a divisão está no corredor transparente, não no tronco.
  local split=math.floor(source.height*.69)
  local x0,y0=math.floor(cx*source.width/4),cy==0 and 0 or split
  local x1,y1=math.floor((cx+1)*source.width/4)-1,cy==0 and split-1 or source.height-1
  local l,t,r,b=x1,y1,x0,y0
  for y=y0,y1 do for x=x0,x1 do if app.pixelColor.rgbaA(source:getPixel(x,y))>128 then
    l=math.min(l,x);t=math.min(t,y);r=math.max(r,x);b=math.max(b,y)
  end end end
  local dh=heights[i];local dw=math.floor((r-l+1)*dh/(b-t+1)+.5)
  local sprite=Sprite(256,224,ColorMode.RGB);local image=Image(256,224,ColorMode.RGB)
  local ox,oy=math.floor((256-dw)/2),224-28-dh
  if oy<0 then oy=0 end
  for y=0,dh-1 do for x=0,dw-1 do
    image:drawPixel(ox+x,oy+y,source:getPixel(l+math.floor(x*(r-l+1)/dw),t+math.floor(y*(b-t+1)/dh)))
  end end
  sprite.cels[1].image=image;sprite:saveAs(root.."/"..name..".aseprite");sprite:saveCopyAs(root.."/"..name..".png");sprite:close()
end
atlas:close();print("Santa Rita: flora exportada")
