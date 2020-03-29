from numaleatorios import Aleatorio

random  = Aleatorio()

class Evento():

    def __init__(self):
        super().__init__()
        self.tipo_evento = ""
        self.tiempo_creacion = 0
        self.tiempo_salida = 0

#Simulacion en minutos

## Creacion de llegadas
tiempo = 0
tiempo_max = 100

eventos = []
while(tiempo < tiempo_max):
    evento = Evento()
    evento.tiempo_creacion = tiempo + 0.5
    evento.tiempo_evento = evento.tiempo_creacion
    evento.tipo_evento = "llegada"
    tiempo = tiempo + 0.5
    eventos.append(evento)

## Inicio de la simulacion 
tiempo = 0
cola_de_espera = []
salidas = []
inspector_ocupado = False
piezas_max = 0

while(tiempo < tiempo_max):
    eventos.sort(key=lambda x:x.tiempo_evento)
    evento = eventos.pop(0) ## Evento proximo
    tiempo = evento.tiempo_evento
    piezas_max = max(piezas_max, len(cola_de_espera))
    if(evento.tipo_evento == "llegada"):
        if(len(cola_de_espera) == 0 and inspector_ocupado == False):
            inspector_ocupado = True
            evento.tiempo_inspeccion = tiempo
            evento.tiempo_evento = tiempo + random.uniforme(0.333,0.5)##Tiempo de inspección 
            evento.tipo_evento ="salida_inspeccion"
            evento.tiempo_salida = evento.tiempo_evento
            eventos.append(evento)
        else:
            cola_de_espera.append(evento)
    elif( evento.tipo_evento == "salida_inspeccion"):
        inspector_ocupado = False
        evento.tiempo_salida = tiempo
        salidas.append(evento)
        if(len(cola_de_espera)>0):
            pieza = cola_de_espera.pop(0)
            inspector_ocupado = True
            pieza.tiempo_inspeccion = tiempo
            pieza.tiempo_evento = tiempo + random.uniforme(0.333,0.5)##Tiempo de inspección 
            pieza.tiempo_salida = pieza.tiempo_evento
            pieza.tipo_evento ="salida_inspeccion"
            eventos.append(pieza)

tiempo_total_inspeccion = 0
for pieza in salidas:
    tiempo_total_inspeccion += pieza.tiempo_salida-pieza.tiempo_inspeccion

print(tiempo_total_inspeccion/tiempo_max) #a

print(piezas_max) # b



