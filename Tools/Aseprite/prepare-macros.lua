local root=assert(app.params.root)
local atlas=app.open(assert(app.params.source));local source=atlas.cels[1].image
for col,name in ipairs({"Asfalto","Grama","Terra"}) do
  local w=math.floor(source.width/4);local h=math.floor(source.height/2)
  local x0=(col-1)*w+8;local y0=math.floor((h-w)/2)+8;local side=w-16
  local sprite=Sprite(128,128,ColorMode.RGB);local image=Image(128,128,ColorMode.RGB)
  for y=0,127 do for x=0,127 do image:drawPixel(x,y,source:getPixel(x0+math.floor(x*side/128),y0+math.floor(y*side/128))) end end
  -- Costura periódica de borda com blend curto, preservando o centro do desenho.
  for axis=0,1 do for i=0,127 do for d=0,3 do
    local a=axis==0 and image:getPixel(d,i) or image:getPixel(i,d)
    local b=axis==0 and image:getPixel(127-d,i) or image:getPixel(i,127-d)
    local weight=(4-d)/8
    local function mix(p,q) return app.pixelColor.rgba(math.floor(app.pixelColor.rgbaR(p)*(1-weight)+app.pixelColor.rgbaR(q)*weight),math.floor(app.pixelColor.rgbaG(p)*(1-weight)+app.pixelColor.rgbaG(q)*weight),math.floor(app.pixelColor.rgbaB(p)*(1-weight)+app.pixelColor.rgbaB(q)*weight),255) end
    if axis==0 then image:drawPixel(d,i,mix(a,b));image:drawPixel(127-d,i,mix(b,a)) else image:drawPixel(i,d,mix(a,b));image:drawPixel(i,127-d,mix(b,a)) end
  end end end
  sprite.cels[1].image=image;sprite:saveAs(root.."/"..name.."_Macro.aseprite");sprite:saveCopyAs(root.."/"..name.."_Macro.png")
  for y=0,3 do for x=0,3 do
    local tile=Sprite(32,32,ColorMode.RGB);local pixels=Image(32,32,ColorMode.RGB)
    for py=0,31 do for px=0,31 do pixels:drawPixel(px,py,image:getPixel(x*32+px,y*32+py)) end end
    tile.cels[1].image=pixels;tile:saveCopyAs(root.."/"..name.."_"..x.."_"..(3-y)..".png");tile:close()
  end end
  sprite:close()
end
atlas:close();print("Santa Rita: 3 materiais macro/48 tiles contínuos")
