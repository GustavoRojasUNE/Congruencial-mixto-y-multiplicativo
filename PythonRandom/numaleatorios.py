#!/usr/bin/env python
import math
import itertools

class Aleatorio:


    def __init__(self, archivo="./series.txt"):
        try:
            f = open(archivo, 'r')
            print("El archivo " + archivo + " fue abierto.")
        except:
            f = open(archivo, 'w')
            print("El archivo " + archivo + " fue creado.")
        f.close()
        self.archivo = archivo
        self.lna = []
        self.i = -1

    def gen_congruencial_mixto(self, x0, m, a, c):
        xn = x0
        i = 0
        numpa = []
        while True:
            i = i + 1
            numpa.append(xn)
            xn = ((a * xn) + c) % m
            if numpa.count(xn) != 0:
                break
            if xn == x0:
                break
        return numpa

    def primos(self, lim):
        lpri = []
        i = 2
        isprimo = True
        limite = int(lim)
        while limite > i:
            isprimo = True
            for primo in lpri:
                if i % primo == 0:
                    isprimo = False
            if isprimo == True:
                lpri.append(i)
            i = i + 1
        return lpri

    def generadora(self, m):
        la = self.primos(m)
        for primo in la:
            if primo % 3 == 0 | primo % 5 == 0:
                la.remove(primo)
        return la

    def generadorc(self, m):
        lc = []
        c = 5
        while c < m:
            lc.append(c)
            c = c + 8
        return lc

    def generadordeclaves(self, b, num):
        xn = 43
        m = (2 ** b)
        la = self.generadora(m)
        lc = self.generadorc(m)
        j = num
        i = 0
        f = open(self.archivo, 'w')
        product = itertools.product(la,lc)
        i = 0
        for combination in product:
            a = combination[0]
            c = combination[1]
            comb = self.gen_congruencial_mixto(xn, m, a, c)
            if (m == len(set(comb))):
                f.write(str(xn) + "," + str(m) + "," + str(a) + "," + str(c) + "\n")
                i += 1
            if(i == num):
                break
        f.close()
        print(str(i) + (" combinaciones generadas con ciclo completo."))
        return "claves.txt"

    def quicksort (self, l, i, d):
        if i < d:
            pivote = l[(i + d) // 2]
            io, do = i, d
            while i <= d:
                while l[i] < pivote: i += 1
                while l[d] > pivote: d -= 1
                if i <= d:
                    l[i], l[d] = l[d], l[i]
                    i += 1
                    d -= 1
            if io < d: self.quicksort(l, io, d)
            if i < do: self.quicksort(l, i, do)
        return l

    def quicksortlista (self, l):
        return self.quicksort(l, 0, len(l)-1)

    def getlistnumale (self, f):
        combinacion = ((str)(f.readline())).split(",")
        x0, m, a, c = 0, 0, 0, 0
        lnum = []
        try:
            x0 = int(combinacion[0])
        except:
            return (x0, m, a, c, lnum, False)
        m = int(combinacion[1])
        a = int(combinacion[2])
        c = int(combinacion[3])
        lnum = (self.gen_congruencial_mixto(x0, m, a, c))
        return (x0, m, a, c, lnum, True)

    def get_na (self):
        if self.lna == []:
            f = open(self.archivo, 'r')
            x0, self.m, a, c, lnum, bool = self.getlistnumale(f)
            self.lna = lnum
        self.i = self.i + 1
        return ((self.lna.pop(0) * 1.0) / self.m)

    def kolmogorov_smirnov(self):
        cont = 0
        f = open(self.archivo, 'r')
        combinaciones = ""
        while True:
            x0, m, a, c, lnum, sige = self.getlistnumale(f)
            if sige == False:
                break
            lnum = (lnum[0:50])#paso 2 y 3
            lnum = [float(n) / m for n in lnum]#paso 4
            fobs = self.quicksortlista(lnum)#paso 5a
            fteo = []
            i = float(0)
            while i <= 50:
                i = i + 1
                fteo.append(i / len(fobs))   #paso 5b
            dmax = 0
            d = 0
            i = 0
            while i < 50:
                d = abs(fobs[i]-fteo[i])
                i = i + 1
                if d > dmax:
                    dmax = d
            if dmax < 0.188:
                combinaciones = combinaciones + (str(x0) + "," + str(m) + "," + str(a) + "," + str(c) + "\n")
                cont = cont + 1
        print(str(cont) + " combinaciones pasaron la prueba Kolmogorov-Smirnov.")
        f.close()
        f = open(self.archivo, 'w')
        f.write(combinaciones)
        f.close()

    def huecos(self):
        cont = 0
        combinaciones = ""
        f = open(self.archivo, 'r')
        while True:
            x0, m, a, c, lnum, sige = self.getlistnumale(f)
            if sige == False:
                break
            lnum = lnum[0:50]
            lnum = [float(n) / m for n in lnum]
            serie10 = []
            alfa = .35
            beta = .65
            teta = beta-alfa
            for n in lnum:
                if (n >= alfa) & (n <= beta):
                    serie10.append(1)
                else:
                    serie10.append(0)
            huecos = []
            i = 0
            mayorhueco = 0
            for n in serie10:
                if n == 0:
                    i = i + 1
                else:
                    if i > mayorhueco:
                        mayorhueco = i
                    huecos.append(i)
                    i = 0
            lenhuecos = []
            i = 0
            while i < mayorhueco:
                lenhuecos.append(huecos.count(i))
                i = i + 1
            th = 0
            fe = []
            i = 0
            for n in lenhuecos:
                th = th + n
                fe.append(th * (teta * (1-teta) ** i))
                i = i + 1
            estcalc = 0.0
            for n in fe:
                estcalc = estcalc + n
            if (len(fe)-1) == 1:
                distribucionchi = 3.8415
            elif (len(fe)-1) == 2:
                distribucionchi = 5.9915
            elif (len(fe)-1) == 3:
                distribucionchi = 7.8147
            elif (len(fe)-1) == 4:
                distribucionchi = 9.4877
            elif (len(fe)-1) == 5:
                distribucionchi = 11.0705
            elif (len(fe)-1) == 6:
                distribucionchi = 12.5916
            elif (len(fe)-1) == 7:
                distribucionchi = 14.0671
            elif (len(fe)-1) == 8:
                distribucionchi = 15.5073
            elif (len(fe)-1) >= 9:
                distribucionchi = 16.9190
            if estcalc < distribucionchi:
                combinaciones = combinaciones + (str(x0) + "," + str(m) + "," + str(a) + "," + str(c) + "\n")
                cont = cont + 1
        print(str(cont) + " combinaciones pasaron la prueba de los Huecos.")
        f.close()
        f = open(self.archivo, 'w')
        f.write(combinaciones)
        f.close()

    def poker(self):
        cont = 0
        f = open(self.archivo, 'r')
        combinaciones = ""
        quintilla = 0
        poker = 1
        full = 2
        tercia = 3
        pares = 4
        par = 5
        diferentes = 6
        probabilidad = [.00010, .00450, .00900, .07200, .10800, .50400, .30240]
        while True:
            cuenta = [0, 0, 0, 0, 0, 0, 0]
            x0, m, a, c, lnum, sige = self.getlistnumale(f)
            if sige == False:
                break
            lnum = lnum[0:50]
            lnum = [float(n) / m for n in lnum]
            lpoker = [str(round(n, 5))[2:] for n in lnum]
            for n in lpoker:
                lmano = []
                i = 1
                while i < 10:
                    lmano.append(n.count(str(i)))
                    i = i + 1
                lmano.append(n.count("0") + (5-len(n)))
                if lmano.count(2) == 1: #par
                    cuenta[par] = cuenta[par] + 1
                elif lmano.count(1) == 5: #diferentes
                    cuenta[diferentes] = cuenta[diferentes] + 1
                elif lmano.count(2) == 2: #2pares
                    cuenta[pares] = cuenta[pares] + 1
                elif lmano.count(3) == 1: #tercia
                    cuenta[tercia] = cuenta[tercia] + 1
                elif lmano.count(3) == 1 & lmano.count(2) == 1: #full
                    cuenta[poker] = cuenta[full] + 1
                elif lmano.count(4) == 1: #poker
                    cuenta[poker] = cuenta[poker] + 1
                elif lmano.count(5) == 1: #quintilla
                    cuenta[quintilla] = cuenta[quintilla] + 1
            lestcalc = []
            estcalc = 0.0
            fesp = 0
            i = 0
            distribucionchi = 12.59
            while i < 7:
                fesp = 50 * probabilidad[i]
                lestcalc.append(((fesp-cuenta[i]) ** 2) / fesp)
                estcalc = estcalc + lestcalc[i]
                i = i + 1
            if estcalc < distribucionchi:
                combinaciones = combinaciones + (str(x0) + "," + str(m) + "," + str(a) + "," + str(c) + "\n")
                cont = cont + 1
        print(str(cont) + " combinaciones pasaron la prueba del Poker.")
        f.close()
        f = open(self.archivo, 'w')
        f.write(combinaciones)
        f.close()

    def corridasmedia(self):
        cont = 0
        f = open(self.archivo, 'r')
        combinaciones = ""
        while True:
            x0, m, a, c, lnum, sige = self.getlistnumale(f)
            if sige == False:
                break
            lnum = (lnum[0:50])
            lnum = [float(n) / m for n in lnum]
            prom = 0
            for n in lnum:
                prom = prom + n
            prom = prom / 50
            loper = []
            n1 = 0
            n2 = 0
            for n in lnum:
                if n >= prom:
                    loper.append(0)
                    n1 = n1 + 1
                else:
                    loper.append(1)
                    n2 = n2 + 1
            a1 = 0
            elemento = loper[0]
            for n in loper:
                if n != elemento:
                    a1 = a1 + 1
                    elemento = n
            ma1 = float(((2 * n1 * n2) / (n1 + n2)) + 1)
            ga1 = float((2 * n1 * n2 * ((2 * n1 * n2)-n1-n2)) / (((n1 + n2) ** 2) * (n1 + n2-1)))
            z = abs(float((a1-ma1) / (math.sqrt(ga1))))
            if z < 2.24:
                combinaciones = combinaciones + (str(x0) + "," + str(m) + "," + str(a) + "," + str(c) + "\n")
                cont = cont + 1
        print(str(cont) + " combinaciones pasaron la prueba del corridas arriba y abajo de la media.")
        f.close()
        f = open(self.archivo, 'w')
        f.write(combinaciones)
        f.close()

    def longuitudcorridas(self):
        cont = 0
        f = open(self.archivo, 'r')
        combinaciones = ""
        while True:
            x0, m, a, c, lnum, sige = self.getlistnumale(f)
            if sige == False:
                break
            lnum = (lnum[0:50])
            lnum = [float(n) / m for n in lnum]
            prom = 0
            num = 50.0
            for n in lnum:
                prom = prom + n
            prom = prom / 50
            loper = []
            n1 = 0
            n2 = 0
            for n in lnum:
                if n <= prom:
                    loper.append(0)
                    n1 = n1 + 1
                else:
                    loper.append(1)
                    n2 = n2 + 1
            corridas = []
            i = 1
            mayorcorrida = 0
            anterior = loper[0]
            tam = 0
            while len(loper) > i:
                tam = tam + 1
                if loper[i] != anterior:
                    if tam > mayorcorrida:
                        mayorcorrida = tam
                    corridas.append(tam)
                    tam = 0
                anterior = loper[i]
                i = i + 1
            corridas.append(tam + 1)
            fe = []
            fo = []
            i = 1
            while i <= mayorcorrida:
                fe.append(2 * num * ((n1 / num) ** i) * ((n2 / num) ** 2))
                fo.append(corridas.count(i))
                i = i + 1
            est = []
            i = 0
            while i < mayorcorrida:
                est.append(((fo[i]-fe[i]) ** 2) / fe[i])
                i = i + 1
            estcalc = 0
            distribucionchi = 0.0
            for n in est:
                estcalc = estcalc + n
            if (len(fe)-1) == 1:
                distribucionchi = 3.8415
            elif (len(fe)-1) == 2:
                distribucionchi = 5.9915
            elif (len(fe)-1) == 3:
                distribucionchi = 7.8147
            elif (len(fe)-1) == 4:
                distribucionchi = 9.4877
            elif (len(fe)-1) == 5:
                distribucionchi = 11.0705
            elif (len(fe)-1) == 6:
                distribucionchi = 12.5916
            elif (len(fe)-1) == 7:
                distribucionchi = 14.0671
            elif (len(fe)-1) == 8:
                distribucionchi = 15.5073
            elif (len(fe)-1) == 9:
                distribucionchi = 16.9190
            if estcalc < distribucionchi:
                combinaciones = combinaciones + (str(x0) + "," + str(m) + "," + str(a) + "," + str(c) + "\n")
                cont = cont + 1
        print(str(cont) + " combinaciones pasaron la prueba de longuitud de corridas.")
        f.close()
        f = open(self.archivo, 'w')
        f.write(combinaciones)
        f.close()

    def montecarlocalculopi(self):
        nlineas = 0
        f = open(self.archivo, 'r')
        nlineas = len(f.readlines())
        f.close()
        f = open(self.archivo, 'r')
        print ("Son " + str(nlineas) + " combinaciones, con cual quieres calcular pi?")
        try:
            linea = int(input())
        except:
            print("Error, se calculara con la combinacion 1.")
        comb = (f.readlines())[int(linea)]
        combinacion = comb.split(",")
        try:
            x0 = int(combinacion[0])
        except:
            print("Error.")
        m = int(combinacion[1])
        a = int(combinacion[2])
        c = int(combinacion[3])
        print(a)
        print (c)
        lnum = (self.gen_congruencial_mixto(x0, m, a, c))
        lnum = [float(n) / m for n in lnum]
        lx = lnum[0::2]
        ly = lnum[1::2]
        i = 0
        dentro = 0
        while i < (m / 2):
            d = math.hypot(lx[i], ly[i])
            if d < 1:
                dentro += 1
            i += 1
        dentro += 1
        print (dentro) #1608-1609
        print (i)
        print (4.0 * (dentro) / (i))

    def trans(self, list):
        if type(list)!= type([0,0]):
            print ("\nError: Las probabilidades deben estar en una lista, ejemplo:")
            print ("[[0,50],[1,25],[2,25]]\n")
            return False
        lprob=[]
        for n in list:
            lprob1=[]
            lprob1.append(n[0])
            lprob1.append(n[1])
            lprob.append(lprob1)
        sum=0.0
        for n in range(0,len(lprob)):
            sum+=lprob[n][1]
        sum=round(sum) #        print sum
        if (sum>.9 and sum<1.1) or sum==100:
            if sum==100:
                lprob=[[n[0],n[1]/100.0] for n in lprob]
            suma=0.0
            for n in range(0,len(lprob)):
                suma+=lprob[n][1]
                lprob[n][1]=suma
        else:
            print ("Error: La lista de probabilidades debe sumar 100% o 1.0")
            return False
        R = self.get_na()
        for n in range(0,len(lprob)):
            if R<lprob[n][1]:
                return R,lprob[n][0]         
        
    def uniforme (self, a, b):
        if a>=b:
            print ("Error a debe ser menor que b")
            return False
        R = self.get_na()
        X = a + R * (b-a)
        return X

    def exponencial(self, B):
        R = self.get_na()
        X = -B * (math.log(R))
        return X

    def merlang(self,B):
        print ("Numero de corridas: ")
        n = int(input())
        multiplicatoria=1.0
        for i in range(0,n):
            R=self.get_na()
            multiplicatoria=multiplicatoria*R
        x=(B/i)*abs(math.log(multiplicatoria))
        return x

    def weibull(self, A, B):
        R = self.get_na()
        ap = 1.0 / A
        X = B * math.pow(abs(math.log(R)), ap)
        return X

    def normal(self):
        R1 = self.get_na()
        V1 = (2 * R1)-1
        R2 = self.get_na()
        V2 = (2 * R2)-1
        W = (math.pow(V1, 2)) + (math.pow(V2, 2))
        if W > 1:
            self.normal()
        else:
            Y = math.sqrt((-2 * math.log(W)) / W)
            X1 = V1 * Y
            X2 = V2 * Y
            return X1,X2

    def normalb(self, u, o):
        R = self.get_na()
        op = math.sqrt(o)
        X = u + (op * R)
        return X

    def lognormal(self, u, o):
        Y = self.normalb(u, o)
        X = (math.pow(math.e, Y))
        return X

    def gamma(self, a):
        cont = 0
        X = 0
        arreglo = []
        print ("Numero de corridas:")
        n = int(input())
        while cont < n:
            E = self.exponencial(1)
            arreglo.append(E)
            cont += 1
        for i in arreglo:
            X += i
        return X

    def gammab(self, a, b):
        Y = self.gamma(a)
        if b > 0:
            X = b * Y
        return X

    def beta (self, a1, a2):
        Y1 = self.gamma(a1)
        Y2 = self.gamma(a2)
        X = Y1 / (Y1 + Y2)
        print ("Variable Beta: " + str(X))

    def triangular(self, c):
        if c<0 or c>1:
            print ("Error: El argumento c deve estar entre 0 y 1")
            return False
        R = self.get_na()
        if R <= c:
            X = math.sqrt(c * R)
            return X
        else:
            X = 1-(math.sqrt((1-c) * (1-R)))
            return X

    def bernoulli(self, p):
        R = self.get_na()
        if R < p:
            X = 1
        else:
            X = 0
        return X

    def uniformedis(self, i, j):
        R = self.get_na()
        X = i + (math.floor((j-i + 1) * R))
        return X

    def binomial(self, t, p):
        cont = 0
        X = 0
        arreglo = []
        while cont < t:
            Y = self.bernoulli(p)
            arreglo.append(Y)            
            cont += 1

        for i in arreglo:
            X += i
        return X

    def geometrica(self, p):
        R = self.get_na()
        X = math.floor((math.log(R) / math.log(1-p)))
        return X

    def bineg(self, s, p):
        cont = 0
        X = 0
        arreglo = []
        while cont < s:
            Y = self.geometrica(p)
            arreglo.append(Y)
            cont += 1
        for i in arreglo:
            X += i
        return X
        
    def poisson(self, y):
        i = 0
        t = 0
        print ("Valor del tiempo")
        TT = int(input())
        while t < TT:
            R = self.get_na()
            xt = -(1 / y) * (math.log(R))
            t = t + xt
            if t <= TT:
                i = i + 1
        return i

    def menuvac (self):
        opc = 0
        while opc != 99:
            print("")
            print("Variables Aleatorias Continuas")
            print("1) Uniforme")
            print("2) Exponencial")
            print("3) m-Erlang")
            print("4) Weibull")
            print("5) Normal")
            print("6) Normal(B)")
            print("7) Lognormal")
            print("8) Gamma")
            print("9) Gamma(B)")
            print("10) Beta")
            print("11) Triangular")
            print("")
            opc = int(input())

            if opc == 1:
                sirve=False
                while sirve==False:
                    print ("Valor de a:")
                    a = float(input())
                    print ("Valor de b:")
                    b = float(input())
                    if b>a:
                        sirve=True
                    else:
                        print ("Dato Erroneo")
                print (self.uniforme(a, b))

            elif opc == 2:
                print ("Valor de B:")
                B = float(input())
                print (self.exponencial(B))

            elif opc == 3:
                print ("Valor de B:")
                B = float(input())
                print (self.merlang(B))

            elif opc == 4:
                print ("Valor de parametro de forma:")
                A = float(input())
                print ("Valor de parametro de escala:")
                B = float(input())
                print (self.weibull(A, B))

            elif opc == 5:
                print (self.normal())

            elif opc == 6:
                print ("Valor de u:")
                u = float(input())
                print ("Valor de o:")
                o = float(input())
                print (self.normalb(u, o))

            elif opc == 7:
                print ("Valor de u:")
                u = float(input())
                print ("Valor de o:")
                o = float(input())
                print (self.lognormal(u, o))

            elif opc == 8:
                print ("Valor de a:")
                a = float(input())
                print( self.gamma(a))

            elif opc == 9:
                print ("Valor de a:")
                a = float(input())
                print ("Valor de b:")
                b = float(input())
                print( self.gammab(a, b))

            elif opc == 10:
                print ("Valor de a1:")
                a1 = float(input())
                print ("Valor de a2:")
                a2 = float(input())
                print (self.beta(a1, a2))

            elif opc == 11:
                print ("Valor de c:")
                sirvec=False
                while sirvec==False:
                    c = float(input())
                    if c>0 and c<1:
                        sirvec=True
                    else:
                        print ("Dato Erroneo")
                print (self.triangular(c))

    def menuvad(self):
        opc = 0
        while opc != 99:
            print ("")
            print ("Variables Aleatorias Discretas")
            print ("1) Bernoulli")
            print ("2) Uniforme")
            print ("3) Binomial")
            print ("4) Geometrica")
            print ("5) Binomial Negativa")
            print ("6) Poisson")
            print ("")
            opc = int(input())

            if opc == 1:
                print ("Valor de media(p):")
                p = float(input())
                print (self.bernoulli(p))

            elif opc == 2:
                print ("Valor de i:")
                i = float(input())
                print ("Valor de j:")
                j = float(input())
                print (self.uniformedis(i, j))

            elif opc == 3:
                print ("Valor de variables bernoulli:")
                t = float(input())
                print ("Valor de media(p):")
                p = float(input())
                print (self.binomial(t, p))

            elif opc == 4:
                print ("Valor de media(p):")
                p = float(input())
                print (self.geometrica(p))

            elif opc == 5:
                print ("Valor de variables geometrica:")
                s = float(input())
                print ("Valor de media(p):")
                p = float(input())
                print (self.bineg(s, p))

            elif opc == 6:
                print ("Valor de de la variable lambda:")
                y = float(input())
                print ("Poisson: "+str(self.poisson(y)))

    def menu(self):
        opc = 0
        while opc != 99:
            print ("Que desea generar: ")
            print ("1) Variables Discretas")
            print ("2) Variables Continuas")
            opc = int(input())
            if opc == 1:
                self.menuvad()
            elif opc == 2:
                self.menuvac()

    
    def pruebas(self):
        self.kolmogorov_smirnov()
        self.huecos()
        self.corridasmedia()
        self.longuitudcorridas()

if __name__ == "__main__":
    aleatorio = Aleatorio("series.txt")
    aleatorio.generadordeclaves(10,1000) # Genera combinaciones de parametros
    aleatorio.pruebas() # Hace las pruebas aleatorias
    # aleatorio.menu() #Menu con las variables aleatorias

