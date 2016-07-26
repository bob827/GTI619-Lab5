Pour initialiser une base de donnees qui ne contient aucune donnees:
 - Executer le script : create.sql
 - Executer le script : insert.sql
 
Le premier script va creer toutes les tables et le secont va mettre des valeurs dans la base de donnees.

Liste des utilisateurs:

Username		Password		GridCard
------------------------------------------------------
admin			123456				1	2	3
								1	72	81	76
								2	55	20	55
								3	90	44	97
------------------------------------------------------
userCercle		123456				1	2	3
								1	81	9	67
								2	55	12	7
								3	76	84	67
------------------------------------------------------
userCarre		123456				1	2	3
								1	32	95	1
								2	66	86	9
								3	29	22	16
------------------------------------------------------


Pour lire les valeurs dans la grid card suivante,
	1	2	3
1	72	81	76
2	55	20	55
3	90	44	97
La valeur qui correspond a (2, 3) = 2e colonne, 3e ligne = 44
La valeur qui correspond a (1, 3) = 1e colonne, 3e ligne = 90