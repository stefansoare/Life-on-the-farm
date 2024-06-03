/*
**
 *Soare Stefan Dan
 * Specializare: Informatica aplicata
 * Tema 7 app 1
 * Acest program redirectioneaza intrarea standard catre un fisier specificat, executa o comanda, 
 * iar apoi redirectioneaza iesirea standard a acestei comenzi catre un alt fisier folosind pipe-uri.
*/

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <fcntl.h>

int main(int argc, char *argv[]) {
    // Verificam daca programul a fost apelat corect in linia de comanda
    if (argc < 5) {
        fprintf(stderr, "Usage: %s CMD1 < FILE CMD2 > FILE\n", argv[0]);
        exit(EXIT_FAILURE);
    }

    // Extragem argumentele din linia de comanda
    char *cmd1 = argv[1];
    char *file_in = argv[3];
    char *cmd2 = argv[4];
    char *file_out = argv[6];

    int pipefd[2];
    pid_t pid1, pid2;

    // Creem un pipe
    if (pipe(pipefd) == -1) {
        perror("pipe");
        exit(EXIT_FAILURE);
    }

    // Creem primul proces fiu pentru CMD1 < FILE
    pid1 = fork();
    if (pid1 == -1) {
        perror("fork");
        exit(EXIT_FAILURE);
    } else if (pid1 == 0) {
        close(pipefd[0]); // Inchidem capatul de citire al pipe-ului

        // Deschidem fisierul de intrare in mod read-only
        int file_in_fd = open(file_in, O_RDONLY);
        if (file_in_fd == -1) {
            perror("open");
            exit(EXIT_FAILURE);
        }

        // Redirectionam stdin catre fisierul de intrare
        dup2(file_in_fd, STDIN_FILENO);
        // Redirectionam stdout catre capatul de scriere al pipe-ului
        dup2(pipefd[1], STDOUT_FILENO);

        // Executam comanda CMD1
        execlp(cmd1, cmd1, NULL);
        perror("execlp");
        exit(EXIT_FAILURE);
    } else {
        // Creem al doilea proces fiu pentru CMD2 > FILE
        pid2 = fork();
        if (pid2 == -1) {
            perror("fork");
            exit(EXIT_FAILURE);
        } else if (pid2 == 0) {
            close(pipefd[1]); // Inchidem capatul de scriere al pipe-ului

            // Deschidem fisierul de iesire in modul write-only, creandu-l daca nu exista si golindu-l daca exista
            int file_out_fd = open(file_out, O_WRONLY | O_CREAT | O_TRUNC, 0666);
            if (file_out_fd == -1) {
                perror("open");
                exit(EXIT_FAILURE);
            }

            // Redirectionam stdin catre capatul de citire al pipe-ului
            dup2(pipefd[0], STDIN_FILENO);
            // Redirectionam stdout catre fisierul de iesire
            dup2(file_out_fd, STDOUT_FILENO);

            // Executam comanda CMD2
            execlp(cmd2, cmd2, NULL);
            perror("execlp");
            exit(EXIT_FAILURE);
        } else {
            // Inchidem capetele pipe-ului in procesul parinte
            close(pipefd[0]);
            close(pipefd[1]);

            // Asteptam ca ambele procese fiu sa se termine
            wait(NULL);
            wait(NULL);
        }
    }

    return 0;
}

/*
**
 *Exemple de compilare si rulare a programului
 *gcc app1t7.c -o app1 --> Compilare
 *./app1 CMD1 < file_in CMD2 > file_out --> Rulare
 */
