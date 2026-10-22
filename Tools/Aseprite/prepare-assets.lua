-- Executar no Aseprite em batch. Recorte/exportação técnica, sem redesenhar o atlas.
local root = assert(app.params.root, "root obrigatório")
local source = assert(app.params.source, "source obrigatório")
local mode = app.params.mode or "npcs"
local atlas = app.open(source)
local original = atlas.cels[1].image
local names = mode == "npcs" and {"tonao","bento","clarice","alcina","dario","mila","celina","nair"}
  or {"Asfalto","Grama","Terra","Calcada","Pedra","Madeira","PisoVerde","Agua"}
if app.params.names then names={};for name in app.params.names:gmatch("[^,]+") do table.insert(names,name) end end
for index, name in ipairs(names) do
  local col, row = (index-1)%4, math.floor((index-1)/4)
  local x0, y0 = math.floor(col*original.width/4), math.floor(row*original.height/2)
  local x1, y1 = math.floor((col+1)*original.width/4)-1, math.floor((row+1)*original.height/2)-1
  if mode == "npcs" then
    local left,top,right,bottom=x1,y1,x0,y0
    for y=y0,y1 do for x=x0,x1 do
      if app.pixelColor.rgbaA(original:getPixel(x,y))>128 then
        left=math.min(left,x);top=math.min(top,y);right=math.max(right,x);bottom=math.max(bottom,y)
      end
    end end
    x0,y0,x1,y1=left,top,right,bottom
  else
    local side=math.min(x1-x0+1,y1-y0+1)
    y0=y0+math.floor((y1-y0+1-side)/2);y1=y0+side-1;x1=x0+side-1
  end
  local size = mode == "npcs" and 64 or 32
  local sprite=Sprite(size,size,ColorMode.RGB)
  local target=Image(size,size,ColorMode.RGB)
  local sw,sh=x1-x0+1,y1-y0+1
  local dw,dh=size,size
  local ox,oy=0,0
  if mode == "npcs" then dh=48;dw=math.floor(sw*dh/sh+.5);ox=math.floor((64-dw)/2);oy=8 end
  for y=0,dh-1 do for x=0,dw-1 do
    target:drawPixel(x+ox,y+oy,original:getPixel(x0+math.floor(x*sw/dw),y0+math.floor(y*sh/dh)))
  end end
  sprite.cels[1].image=target
  local basename=mode=="npcs" and "Npc_"..name or name
  sprite:saveAs(root.."/"..basename..".aseprite")
  sprite:saveCopyAs(root.."/"..basename..".png")
  sprite:close()
end
atlas:close()
if mode ~= "npcs" then
  for _,name in ipairs({"AsfaltoH","AsfaltoV","PisoCreme","PisoCinza","Areia"}) do
    local sourceName=name:sub(1,7)=="Asfalto" and "Asfalto" or name=="Areia" and "Terra" or "PisoVerde"
    local sprite=app.open(root.."/"..sourceName..".aseprite")
    local image=sprite.cels[1].image
    for y=0,31 do for x=0,31 do
      local p=image:getPixel(x,y)
      if (name=="AsfaltoH" and (y==14 or y==17) and x<22) or (name=="AsfaltoV" and (x==14 or x==17) and y<22) then
        image:drawPixel(x,y,app.pixelColor.rgba(181,150,64,255))
      elseif name=="PisoCreme" or name=="PisoCinza" or name=="Areia" then
        local r,g,b=app.pixelColor.rgbaR(p),app.pixelColor.rgbaG(p),app.pixelColor.rgbaB(p)
        if name=="PisoCreme" then r=math.min(255,r*1.03);g=g*.95;b=b*.82
        elseif name=="PisoCinza" then local v=(r+g+b)/3*.76;r=v;g=v*1.02;b=v*1.01
        else r=math.min(255,r*1.12);g=math.min(255,g*1.15);b=b*1.05 end
        image:drawPixel(x,y,app.pixelColor.rgba(math.floor(r),math.floor(g),math.floor(b),255))
      end
    end end
    sprite:saveAs(root.."/"..name..".aseprite");sprite:saveCopyAs(root.."/"..name..".png");sprite:close()
  end
end
print("Santa Rita: 8 assets exportados ("..mode..")")
